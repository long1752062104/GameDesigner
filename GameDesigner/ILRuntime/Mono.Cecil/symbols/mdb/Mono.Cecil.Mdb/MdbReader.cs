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
using ILRuntime.Mono.CompilerServices.SymbolWriter;
using System;
using System.Collections.Generic;
using System.IO;

namespace ILRuntime.Mono.Cecil.Mdb
{

    public sealed class MdbReaderProvider : ISymbolReaderProvider
    {

        public ISymbolReader GetSymbolReader(ModuleDefinition module, string fileName)
        {
            Mixin.CheckModule(module);
            Mixin.CheckFileName(fileName);

            return new MdbReader(module, MonoSymbolFile.ReadSymbolFile(Mixin.GetMdbFileName(fileName)));
        }

        public ISymbolReader GetSymbolReader(ModuleDefinition module, Stream symbolStream)
        {
            Mixin.CheckModule(module);
            Mixin.CheckStream(symbolStream);

            return new MdbReader(module, MonoSymbolFile.ReadSymbolFile(symbolStream));
        }
    }

    public sealed class MdbReader : ISymbolReader
    {

        readonly ModuleDefinition module;
        readonly MonoSymbolFile symbol_file;
        readonly Dictionary<string, Document> documents;

        public MdbReader(ModuleDefinition module, MonoSymbolFile symFile)
        {
            this.module = module;
            symbol_file = symFile;
            documents = new Dictionary<string, Document>();
        }

        public ISymbolWriterProvider GetWriterProvider()
        {
            return new MdbWriterProvider();
        }

        public bool ProcessDebugHeader(ImageDebugHeader header)
        {
            return symbol_file.Guid == module.Mvid;
        }

        public MethodDebugInformation Read(MethodDefinition method)
        {
            MetadataToken method_token = method.MetadataToken;
            MethodEntry entry = symbol_file.GetMethodByToken(method_token.ToInt32());
            if (entry == null)
                return null;

            MethodDebugInformation info = new MethodDebugInformation(method);
            info.code_size = ReadCodeSize(method);

            ScopeDebugInformation[] scopes = ReadScopes(entry, info);
            ReadLineNumbers(entry, info);
            ReadLocalVariables(entry, scopes);

            return info;
        }

        static int ReadCodeSize(MethodDefinition method)
        {
            return method.Module.Read(method, (m, reader) => reader.ReadCodeSize(m));
        }

        static void ReadLocalVariables(MethodEntry entry, ScopeDebugInformation[] scopes)
        {
            LocalVariableEntry[] locals = entry.GetLocals();

            foreach (LocalVariableEntry local in locals)
            {
                VariableDebugInformation variable = new VariableDebugInformation(local.Index, local.Name);

                int index = local.BlockIndex;
                if (index < 0 || index >= scopes.Length)
                    continue;

                ScopeDebugInformation scope = scopes[index];
                if (scope == null)
                    continue;

                scope.Variables.Add(variable);
            }
        }

        void ReadLineNumbers(MethodEntry entry, MethodDebugInformation info)
        {
            LineNumberTable table = entry.GetLineNumberTable();

            info.sequence_points = new Collection<SequencePoint>(table.LineNumbers.Length);

            for (int i = 0; i < table.LineNumbers.Length; i++)
            {
                LineNumberEntry line = table.LineNumbers[i];
                if (i > 0 && table.LineNumbers[i - 1].Offset == line.Offset)
                    continue;

                info.sequence_points.Add(LineToSequencePoint(line));
            }
        }

        Document GetDocument(SourceFileEntry file)
        {
            string file_name = file.FileName;

            if (documents.TryGetValue(file_name, out Document document))
                return document;

            document = new Document(file_name)
            {
                Hash = file.Checksum,
            };

            documents.Add(file_name, document);

            return document;
        }

        static ScopeDebugInformation[] ReadScopes(MethodEntry entry, MethodDebugInformation info)
        {
            CodeBlockEntry[] blocks = entry.GetCodeBlocks();
            ScopeDebugInformation[] scopes = new ScopeDebugInformation[blocks.Length + 1];

            info.scope = scopes[0] = new ScopeDebugInformation
            {
                Start = new InstructionOffset(0),
                End = new InstructionOffset(info.code_size),
            };

            foreach (CodeBlockEntry block in blocks)
            {
                if (block.BlockType != CodeBlockEntry.Type.Lexical && block.BlockType != CodeBlockEntry.Type.CompilerGenerated)
                    continue;

                ScopeDebugInformation scope = new ScopeDebugInformation();
                scope.Start = new InstructionOffset(block.StartOffset);
                scope.End = new InstructionOffset(block.EndOffset);

                scopes[block.Index + 1] = scope;

                if (!AddScope(info.scope.Scopes, scope))
                    info.scope.Scopes.Add(scope);
            }

            return scopes;
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

        SequencePoint LineToSequencePoint(LineNumberEntry line)
        {
            SourceFileEntry source = symbol_file.GetSourceFile(line.File);
            return new SequencePoint(line.Offset, GetDocument(source))
            {
                StartLine = line.Row,
                EndLine = line.EndRow,
                StartColumn = line.Column,
                EndColumn = line.EndColumn,
            };
        }

        public void Dispose()
        {
            symbol_file.Dispose();
        }
    }

    static class MethodEntryExtensions
    {

        public static bool HasColumnInfo(this MethodEntry entry)
        {
            return (entry.MethodFlags & MethodEntry.Flags.ColumnsInfoIncluded) != 0;
        }

        public static bool HasEndInfo(this MethodEntry entry)
        {
            return (entry.MethodFlags & MethodEntry.Flags.EndInfoIncluded) != 0;
        }
    }
}
