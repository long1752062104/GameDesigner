using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct NetShareOperationBind : ISerialize<Net.Share.Operation>
	{
		public void Write(Net.Share.Operation value, Segment strem)
		{
			int pos = strem.Position;
			strem.Position += 2;
			byte[] bits = new byte[2];
			if(value.cmd != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				strem.WriteValue(value.cmd);
			}
			if(value.cmd1 != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				strem.WriteValue(value.cmd1);
			}
			if(value.cmd2 != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				strem.WriteValue(value.cmd2);
			}
			if (!string.IsNullOrEmpty(value.name))
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				strem.WriteValue(value.name);
			}
			if(value.position != null)
			{
				NetConvertBase.SetBit(ref bits[0], 5, true);
				NetVector3Bind bind = new NetVector3Bind();
				bind.Write(value.position, strem);
			}
			if(value.rotation != null)
			{
				NetConvertBase.SetBit(ref bits[0], 6, true);
				NetQuaternionBind bind = new NetQuaternionBind();
				bind.Write(value.rotation, strem);
			}
			if(value.direction != null)
			{
				NetConvertBase.SetBit(ref bits[0], 7, true);
				NetVector3Bind bind = new NetVector3Bind();
				bind.Write(value.direction, strem);
			}
			if(value.index != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 8, true);
				strem.WriteValue(value.index);
			}
			if(value.index1 != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 1, true);
				strem.WriteValue(value.index1);
			}
			if(value.index2 != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 2, true);
				strem.WriteValue(value.index2);
			}
			if(value.buffer != null)
			{
				NetConvertBase.SetBit(ref bits[1], 3, true);
				strem.WriteArray(value.buffer);
			}
			if (!string.IsNullOrEmpty(value.name1))
			{
				NetConvertBase.SetBit(ref bits[1], 4, true);
				strem.WriteValue(value.name1);
			}
			if (!string.IsNullOrEmpty(value.name2))
			{
				NetConvertBase.SetBit(ref bits[1], 5, true);
				strem.WriteValue(value.name2);
			}
			int pos1 = strem.Position;
			strem.Position = pos;
			strem.Write(bits, 0, 2);
			strem.Position = pos1;
		}

		public Net.Share.Operation Read(Segment strem)
		{
			byte[] bits = strem.Read(2);
			var value = new Net.Share.Operation();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.cmd = strem.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.cmd1 = strem.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.cmd2 = strem.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 4))
				value.name = strem.ReadValue<String>();
			if(NetConvertBase.GetBit(bits[0], 5))
			{
				NetVector3Bind bind = new NetVector3Bind();
				value.position = bind.Read(strem);
			}
			if(NetConvertBase.GetBit(bits[0], 6))
			{
				NetQuaternionBind bind = new NetQuaternionBind();
				value.rotation = bind.Read(strem);
			}
			if(NetConvertBase.GetBit(bits[0], 7))
			{
				NetVector3Bind bind = new NetVector3Bind();
				value.direction = bind.Read(strem);
			}
			if(NetConvertBase.GetBit(bits[0], 8))
				value.index = strem.ReadValue<Int32>();
			if(NetConvertBase.GetBit(bits[1], 1))
				value.index1 = strem.ReadValue<Int32>();
			if(NetConvertBase.GetBit(bits[1], 2))
				value.index2 = strem.ReadValue<Int32>();
			if(NetConvertBase.GetBit(bits[1], 3))
				value.buffer = strem.ReadArray<System.Byte>();
			if(NetConvertBase.GetBit(bits[1], 4))
				value.name1 = strem.ReadValue<String>();
			if(NetConvertBase.GetBit(bits[1], 5))
				value.name2 = strem.ReadValue<String>();
			return value;
		}
	}
}

namespace Binding
{
	public struct NetShareOperationArrayBind : ISerialize<Net.Share.Operation[]>
	{
		public void Write(Net.Share.Operation[] value, Segment strem)
		{
			int count = value.Length;
			strem.WriteValue(count);
			if (count == 0) return;
			NetShareOperationBind bind = new NetShareOperationBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public Net.Share.Operation[] Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new Net.Share.Operation[count];
			if (count == 0) return value;
			NetShareOperationBind bind = new NetShareOperationBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(strem);
			return value;
		}
	}
}

namespace Binding
{
	public struct NetShareOperationGenericBind : ISerialize<List<Net.Share.Operation>>
	{
		public void Write(List<Net.Share.Operation> value, Segment strem)
		{
			int count = value.Count;
			strem.WriteValue(count);
			if (count == 0) return;
			NetShareOperationBind bind = new NetShareOperationBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public List<Net.Share.Operation> Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new List<Net.Share.Operation>();
			if (count == 0) return value;
			NetShareOperationBind bind = new NetShareOperationBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(strem));
			return value;
		}
	}
}
