using System;
using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
	public struct NetVector3Bind : ISerialize<Net.Vector3>, ISerialize
	{
		public void Write(Net.Vector3 value, Segment stream)
		{
			int pos = stream.Position;
			stream.Position += 1;
			byte[] bits = new byte[1];
			if(value.x != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				stream.WriteValue(value.x);
			}
			if(value.y != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				stream.WriteValue(value.y);
			}
			if(value.z != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				stream.WriteValue(value.z);
			}
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 1);
			stream.Position = pos1;
		}

		public Net.Vector3 Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new Net.Vector3();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.z = stream.ReadValue<Single>();
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Vector3)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetVector3ArrayBind : ISerialize<Net.Vector3[]>, ISerialize
	{
		public void Write(Net.Vector3[] value, Segment stream)
		{
			int count = value.Length;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new NetVector3Bind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Net.Vector3[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new Net.Vector3[count];
			if (count == 0) return value;
			var bind = new NetVector3Bind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Vector3[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetVector3GenericBind : ISerialize<List<Net.Vector3>>, ISerialize
	{
		public void Write(List<Net.Vector3> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new NetVector3Bind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Net.Vector3> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<Net.Vector3>();
			if (count == 0) return value;
			var bind = new NetVector3Bind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Net.Vector3>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
