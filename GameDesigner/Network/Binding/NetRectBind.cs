using System;
using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
	public struct NetRectBind : ISerialize<Net.Rect>, ISerialize
	{
		public void Write(Net.Rect value, Segment stream)
		{
			int pos = stream.Position;
			stream.Position += 1;
			byte[] bits = new byte[1];
			if (value.x != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				stream.WriteValue(value.x);
			}
			if (value.y != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				stream.WriteValue(value.y);
			}
			if (value.width != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				stream.WriteValue(value.width);
			}
			if (value.height != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				stream.WriteValue(value.height);
			}
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 1);
			stream.Position = pos1;
		}

		public Net.Rect Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new Net.Rect();
			if (NetConvertBase.GetBit(bits[0], 1))
				value.x = stream.ReadValue<float>();
			if (NetConvertBase.GetBit(bits[0], 2))
				value.y = stream.ReadValue<float>();
			if (NetConvertBase.GetBit(bits[0], 3))
				value.width = stream.ReadValue<float>();
			if (NetConvertBase.GetBit(bits[0], 4))
				value.height = stream.ReadValue<float>();
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Rect)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetRectArrayBind : ISerialize<Net.Rect[]>, ISerialize
	{
		public void Write(Net.Rect[] value, Segment stream)
		{
			int count = value.Length;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new NetRectBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Net.Rect[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new Net.Rect[count];
			if (count == 0) return value;
			var bind = new NetRectBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Rect[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetRectGenericBind : ISerialize<List<Net.Rect>>, ISerialize
	{
		public void Write(List<Net.Rect> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new NetRectBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Net.Rect> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<Net.Rect>();
			if (count == 0) return value;
			var bind = new NetRectBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Net.Rect>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
