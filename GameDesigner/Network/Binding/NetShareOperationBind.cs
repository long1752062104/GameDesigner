using System;
using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
	public struct NetShareOperationBind : ISerialize<Net.Share.Operation>, ISerialize
	{
		public void Write(Net.Share.Operation value, Segment stream)
		{
			int pos = stream.Position;
			stream.Position += 2;
			byte[] bits = new byte[2];
			if(value.cmd != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				stream.WriteValue(value.cmd);
			}
			if(value.cmd1 != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				stream.WriteValue(value.cmd1);
			}
			if(value.cmd2 != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				stream.WriteValue(value.cmd2);
			}
			if (!string.IsNullOrEmpty(value.name))
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				stream.WriteValue(value.name);
			}
			if(value.position != default)
			{
				NetConvertBase.SetBit(ref bits[0], 5, true);
				var bind = new NetVector3Bind();
				bind.Write(value.position, stream);
			}
			if(value.rotation != Net.Quaternion.zero)
			{
				NetConvertBase.SetBit(ref bits[0], 6, true);
				var bind = new NetQuaternionBind();
				bind.Write(value.rotation, stream);
			}
			if(value.direction != default)
			{
				NetConvertBase.SetBit(ref bits[0], 7, true);
				var bind = new NetVector3Bind();
				bind.Write(value.direction, stream);
			}
			if(value.index != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 8, true);
				stream.WriteValue(value.index);
			}
			if(value.index1 != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 1, true);
				stream.WriteValue(value.index1);
			}
			if(value.index2 != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 2, true);
				stream.WriteValue(value.index2);
			}
			if(value.buffer != null)
			{
				NetConvertBase.SetBit(ref bits[1], 3, true);
				stream.WriteArray(value.buffer);
			}
			if (!string.IsNullOrEmpty(value.name1))
			{
				NetConvertBase.SetBit(ref bits[1], 4, true);
				stream.WriteValue(value.name1);
			}
			if (!string.IsNullOrEmpty(value.name2))
			{
				NetConvertBase.SetBit(ref bits[1], 5, true);
				stream.WriteValue(value.name2);
			}
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 2);
			stream.Position = pos1;
		}

		public Net.Share.Operation Read(Segment stream)
		{
			byte[] bits = stream.Read(2);
			var value = new Net.Share.Operation();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.cmd = stream.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.cmd1 = stream.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.cmd2 = stream.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 4))
				value.name = stream.ReadValue<String>();
			if(NetConvertBase.GetBit(bits[0], 5))
			{
				var bind = new NetVector3Bind();
				value.position = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 6))
			{
				var bind = new NetQuaternionBind();
				value.rotation = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 7))
			{
				var bind = new NetVector3Bind();
				value.direction = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 8))
				value.index = stream.ReadValue<Int32>();
			if(NetConvertBase.GetBit(bits[1], 1))
				value.index1 = stream.ReadValue<Int32>();
			if(NetConvertBase.GetBit(bits[1], 2))
				value.index2 = stream.ReadValue<Int32>();
			if(NetConvertBase.GetBit(bits[1], 3))
				value.buffer = stream.ReadArray<System.Byte>();
			if(NetConvertBase.GetBit(bits[1], 4))
				value.name1 = stream.ReadValue<String>();
			if(NetConvertBase.GetBit(bits[1], 5))
				value.name2 = stream.ReadValue<String>();
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Share.Operation)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetShareOperationArrayBind : ISerialize<Net.Share.Operation[]>, ISerialize
	{
		public void Write(Net.Share.Operation[] value, Segment stream)
		{
			int count = value.Length;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new NetShareOperationBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Net.Share.Operation[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new Net.Share.Operation[count];
			if (count == 0) return value;
			var bind = new NetShareOperationBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Share.Operation[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetShareOperationGenericBind : ISerialize<List<Net.Share.Operation>>, ISerialize
	{
		public void Write(List<Net.Share.Operation> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new NetShareOperationBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Net.Share.Operation> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<Net.Share.Operation>();
			if (count == 0) return value;
			var bind = new NetShareOperationBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Net.Share.Operation>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
