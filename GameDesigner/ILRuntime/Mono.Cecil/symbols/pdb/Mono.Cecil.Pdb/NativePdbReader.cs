//
// Author:
//   Jb Evain (jbevain@gmail.com)
//
// Copyright (c) 2008 - 2015 Jb Evain
// Copyright (c) 2008 - 2011 Novell, Inc.
//
// Licensed under the MIT/X11 license.
//

using ILRuntime.Mono.Cecil.Cil;
using ILRuntime.Mono.Collections.Generic;
using Microsoft.Cci.Pdb;
using System;
using System.Collections.Generic;
using System.IO;

namespace ILRuntime.Mono.Cecil.Pdb
{

    public class NativePdbReader : ISymbolReader
    {

        int age;
        Guid guid;

        readonly Disposable<Stream> pdb_file;
        readonly Dictionary<string, Document> documents = new Dictionary<string, Document>();
        readonly Dictionary<uint, PdbFunction> functions = new Dictionary<uint, PdbFunction>();
        readonly Dictionary<PdbScope, ImportDebugInformation> imports = new Dictionary<PdbScope, ImportDebugInformation>();

        internal NativePdbReader(Disposable<Stream> file)
        {
            pdb_file = file;
        }

        public ISymbolWriterProvider GetWriterProvider()
        {
            return new NativePdbWriterProvider();
        }

        /*
		uint Magic = 0x53445352;
		Guid Signature;
		uint Age;
		string FileName;
		 */

        public bool ProcessDebugHeader(ImageDebugHeader header)
        {
            if (!header.HasEntries)
                return false;

            ImageDebugHeaderEntry entry = header.GetCodeViewEntry();
            if (entry == null)
                return false;

            ImageDebugDirectory directory = entry.Directory;

            if (directory.Type != ImageDebugType.CodeView)
                return false;

            byte[] data = entry.Data;

            if (data.Length < 24)
                return false;

            int magic = ReadInt32(data, 0);
            if (magic != 0x53445352)
                return false;

            byte[] guid_bytes = new byte[16];
            Buffer.BlockCopy(data, 4, guid_bytes, 0, 16);

            guid = new Guid(guid_bytes);
            age = ReadInt32(data, 20);

            return PopulateFunctions();
        }

        static int ReadInt32(byte[] bytes, int start)
        {
            return (bytes[start]
                | (bytes[start + 1] << 8)
                | (bytes[start + 2] << 16)
                | (bytes[start + 3] << 24));
        }

        bool PopulateFunctions()
        {
            using (pdb_file)
            {
                PdbInfo info = PdbFile.LoadFunctions(pdb_file.value);

                if (guid != info.Guid)
                    return false;

                foreach (PdbFunction function in info.Functions)
                    functions.Add(function.token, function);
            }

            return true;
        }

        public MethodDebugInformation Read(MethodDefinition method)
        {
            MetadataToken method_token = method.MetadataToken;

            if (!functions.TryGetValue(method_token.ToUInt32(), out PdbFunction function))
                return null;

            MethodDebugInformation symbol = new MethodDebugInformation(method);

            ReadSequencePoints(function, symbol);

            symbol.scope = !function.scopes.IsNullOrEmpty()
                ? ReadScopeAndLocals(function.scopes[0], symbol)
                : new ScopeDebugInformation { Start = new InstructionOffset(0), End = new InstructionOffset((int)function.length) };

            if (function.tokenOfMethodWhoseUsingInfoAppliesToThisMethod != method.MetadataToken.ToUInt32() && function.tokenOfMethodWhoseUsingInfoAppliesToThisMethod != 0)
                symbol.scope.import = GetImport(function.tokenOfMethodWhoseUsingInfoAppliesToThisMethod, method.Module);

            if (function.scopes.Length > 1)
            {
                for (int i = 1; i < function.scopes.Length; i++)
                {
                    ScopeDebugInformation s = ReadScopeAndLocals(function.scopes[i], symbol);
                    if (!AddScope(symbol.scope.Scopes, s))
                        symbol.scope.Scopes.Add(s);
                }
            }

            if (function.iteratorScopes != null)
            {
                StateMachineScopeDebugInformation state_machine = new StateMachineScopeDebugInformation();

                foreach (ILocalScope iterator_scope in function.iteratorScopes)
                {
                    state_machine.Scopes.Add(new StateMachineScope((int)iterator_scope.Offset, (int)(iterator_scope.Offset + iterator_scope.Length + 1)));
                }

                symbol.CustomDebugInformations.Add(state_machine);
            }

            if (function.synchronizationInformation != null)
            {
                AsyncMethodBodyDebugInformation async_debug_info = new AsyncMethodBodyDebugInformation((int)function.synchronizationInformation.GeneratedCatchHandlerOffset);

                foreach (PdbSynchronizationPoint synchronization_point in function.synchronizationInformation.synchronizationPoints)
                {
                    async_debug_info.Yields.Add(new InstructionOffset((int)synchronization_point.SynchronizeOffset));
                    async_debug_info.Resumes.Add(new InstructionOffset((int)synchronization_point.ContinuationOffset));
                    async_debug_info.ResumeMethods.Add(method);
                }

                symbol.CustomDebugInformations.Add(async_debug_info);

                symbol.StateMachineKickOffMethod = (MethodDefinition)method.Module.LookupToken((int)function.synchronizationInformation.kickoffMethodToken);
            }

            return symbol;
        }

        Collection<ScopeDebugInformation> ReadScopeAndLocals(PdbScope[] scopes, MethodDebugInformation info)
        {
            Collection<ScopeDebugInformation> symbols = new Collection<ScopeDebugInformation>(scopes.Length);

            foreach (PdbScope scope in scopes)
                if (scope != null)
                    symbols.Add(ReadScopeAndLocals(scope, info));

            return symbols;
        }

        ScopeDebugInformation ReadScopeAndLocals(PdbScope scope, MethodDebugInformation info)
        {
            ScopeDebugInformation parent = new ScopeDebugInformation();
            parent.Start = new InstructionOffset((int)scope.offset);
            parent.End = new InstructionOffset((int)(scope.offset + scope.length));

            if (!scope.slots.IsNullOrEmpty())
            {
                parent.variables = new Collection<VariableDebugInformation>(scope.slots.Length);

                foreach (PdbSlot slot in scope.slots)
                {
                    if ((slot.flags & 1) != 0) // parameter names
                        continue;

                    int index = (int)slot.slot;
                    VariableDebugInformation variable = new VariableDebugInformation(index, slot.name);
                    if ((slot.flags & 4) != 0)
                        variable.IsDebuggerHidden = true;
                    parent.variables.Add(variable);
                }
            }

            if (!scope.constants.IsNullOrEmpty())
            {
                parent.constants = new Collection<ConstantDebugInformation>(scope.constants.Length);

                foreach (PdbConstant constant in scope.constants)
                {
                    TypeReference type = info.Method.Module.Read(constant, (c, r) => r.ReadConstantSignature(new MetadataToken(c.token)));
                    object value = constant.value;

                    // Object "null" is encoded as integer
                    if (type != null && !type.IsValueType && value is int && (int)value == 0)
                        value = null;

                    parent.constants.Add(new ConstantDebugInformation(constant.name, type, value));
                }
            }

            if (!scope.usedNamespaces.IsNullOrEmpty())
            {
                if (imports.TryGetValue(scope, out ImportDebugInformation import))
                {
                    parent.import = import;
                }
                else
                {
                    import = GetImport(scope, info.Method.Module);
                    imports.Add(scope, import);
                    parent.import = import;
                }
            }

            parent.scopes = ReadScopeAndLocals(scope.scopes, info);

            return parent;
        }

        static bool AddScope(Collection<ScopeDebugInformation> scopes, ScopeDebugInformation scope)
        {
            foreach (ScopeDebugInformation sub_scope in scopes)
            {
                if (sub_scope.HasScopes && AddScope(sub_scope.Scopes, scope))
                    return true;

                if (scope.Start.Offset >= sub_scope.Start.Offset && scope.End.Offset <= sub_scope.End.Offset)
                {
                    sub_scope.Scopes.Add(scope);
                    return true;
                }
            }

            return false;
        }

        ImportDebugInformation GetImport(uint token, ModuleDefinition module)
        {
            if (!functions.TryGetValue(token, out PdbFunction function))
                return null;

            if (function.scopes.Length != 1)
                return null;

            PdbScope scope = function.scopes[0];

            if (imports.TryGetValue(scope, out ImportDebugInformation import))
                return import;

            import = GetImport(scope, module);
            imports.Add(scope, import);
            return import;
        }

        static ImportDebugInformation GetImport(PdbScope scope, ModuleDefinition module)
        {
            if (scope.usedNamespaces.IsNullOrEmpty())
                return null;

            ImportDebugInformation import = new ImportDebugInformation();

            foreach (string used_namespace in scope.usedNamespaces)
            {
                if (string.IsNullOrEmpty(used_namespace))
                    continue;

                ImportTarget target = null;
                string value = used_namespace.Substring(1);
                switch (used_namespace[0])
                {
                    case 'U':
                        target = new ImportTarget(ImportTargetKind.ImportNamespace) { @namespace = value };
                        break;
                    case 'T':
                        {
                            TypeReference type = TypeParser.ParseType(module, value);
                            if (type != null)
                                target = new ImportTarget(ImportTargetKind.ImportType) { type = type };
                            break;
                        }
                    case 'A':
                        int index = used_namespace.IndexOf(' ');
                        if (index < 0)
                        {
                            target = new ImportTarget(ImportTargetKind.ImportNamespace) { @namespace = used_namespace };
                            break;
                        }
                        string alias_value = used_namespace.Substring(1, index - 1);
                        string alias_target_value = used_namespace.Substring(index + 2);
                        switch (used_namespace[index + 1])
                        {
                            case 'U':
                                target = new ImportTarget(ImportTargetKind.DefineNamespaceAlias) { alias = alias_value, @namespace = alias_target_value };
                                break;
                            case 'T':
                                TypeReference type = TypeParser.ParseType(module, alias_target_value);
                                if (type != null)
                                    target = new ImportTarget(ImportTargetKind.DefineTypeAlias) { alias = alias_value, type = type };
                                break;
                        }
                        break;
                    case '*':
                        target = new ImportTarget(ImportTargetKind.ImportNamespace) { @namespace = value };
                        break;
                    case '@':
                        if (!value.StartsWith("P:"))
                            continue;

                        target = new ImportTarget(ImportTargetKind.ImportNamespace) { @namespace = value.Substring(2) };
                        break;
                }

                if (target != null)
                    import.Targets.Add(target);
            }

            return import;
        }

        void ReadSequencePoints(PdbFunction function, MethodDebugInformation info)
        {
            if (function.lines == null)
                return;

            info.sequence_points = new Collection<SequencePoint>();

            foreach (PdbLines lines in function.lines)
                ReadLines(lines, info);
        }

        void ReadLines(PdbLines lines, MethodDebugInformation info)
        {
            Document document = GetDocument(lines.file);

            foreach (PdbLine line in lines.lines)
                ReadLine(line, document, info);
        }

        static void ReadLine(PdbLine line, Document document, MethodDebugInformation info)
        {
            SequencePoint sequence_point = new SequencePoint((int)line.offset, document);
            sequence_point.StartLine = (int)line.lineBegin;
            sequence_point.StartColumn = line.colBegin;
            sequence_point.EndLine = (int)line.lineEnd;
            sequence_point.EndColumn = line.colEnd;

            info.sequence_points.Add(sequence_point);
        }

        Document GetDocument(PdbSource source)
        {
            string name = source.name;
            if (documents.TryGetValue(name, out Document document))
                return document;

            document = new Document(name)
            {
                LanguageGuid = source.language,
                LanguageVendorGuid = source.vendor,
                TypeGuid = source.doctype,
                HashAlgorithmGuid = source.checksumAlgorithm,
                Hash = source.checksum,
            };
            documents.Add(name, document);
            return document;
        }

        public void Dispose()
        {
            pdb_file.Dispose();
        }
    }
}
