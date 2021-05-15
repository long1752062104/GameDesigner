// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Cci.Pdb
{
  /// <summary />
  internal class PdbConstant
  {
    internal string name;
    internal uint token;
    internal object value;

    internal PdbConstant(string name, uint token, object value)
    {
      this.name = name;
      this.token = token;
      this.value = value;
    }

    internal PdbConstant(BitAccess bits)
    {
      bits.ReadUInt32(out token);
      bits.ReadUInt8(out byte tag1);
      bits.ReadUInt8(out byte tag2);
      if (tag2 == 0)
      {
        value = tag1;
      }
      else if (tag2 == 0x80)
      {
        switch (tag1)
        {
          case 0x00: //sbyte
            sbyte sb;
            bits.ReadInt8(out sb);
            value = sb;
            break;
          case 0x01: //short
            short s;
            bits.ReadInt16(out s);
            value = s;
            break;
          case 0x02: //ushort
            ushort us;
            bits.ReadUInt16(out us);
            value = us;
            break;
          case 0x03: //int
            int i;
            bits.ReadInt32(out i);
            value = i;
            break;
          case 0x04: //uint
            uint ui;
            bits.ReadUInt32(out ui);
            value = ui;
            break;
          case 0x05: //float
            value = bits.ReadFloat();
            break;
          case 0x06: //double
            value = bits.ReadDouble();
            break;
          case 0x09: //long
            long sl;
            bits.ReadInt64(out sl);
            value = sl;
            break;
          case 0x0a: //ulong
            ulong ul;
            bits.ReadUInt64(out ul);
            value = ul;
            break;
          case 0x10: //string
            string str;
            bits.ReadBString(out str);
            value = str;
            break;
          case 0x19: //decimal
            value = bits.ReadDecimal();
            break;
          default:
            //TODO: error
            break;
        }
      }
      else
      {
        //TODO: error
      }
      bits.ReadCString(out name);
    }
  }
}
