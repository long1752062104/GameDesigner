using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct NetRectBind : ISerialize<Net.Rect>, ISerialize
	{
		public void Write(Net.Rect value, Segment stream)
		{
			int pos = stream.Position;
			stream.Position += 2;
			byte[] bits = new byte[2];
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
			if(value.position != null)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.position, stream);
			}
			if(value.center != null)
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.center, stream);
			}
			if(value.min != null)
			{
				NetConvertBase.SetBit(ref bits[0], 5, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.min, stream);
			}
			if(value.max != null)
			{
				NetConvertBase.SetBit(ref bits[0], 6, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.max, stream);
			}
			if(value.width != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 7, true);
				stream.WriteValue(value.width);
			}
			if(value.height != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 8, true);
				stream.WriteValue(value.height);
			}
			if(value.size != null)
			{
				NetConvertBase.SetBit(ref bits[1], 1, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.size, stream);
			}
			if(value.xMin != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 2, true);
				stream.WriteValue(value.xMin);
			}
			if(value.yMin != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 3, true);
				stream.WriteValue(value.yMin);
			}
			if(value.xMax != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 4, true);
				stream.WriteValue(value.xMax);
			}
			if(value.yMax != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 5, true);
				stream.WriteValue(value.yMax);
			}
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 2);
			stream.Position = pos1;
		}

		public Net.Rect Read(Segment stream)
		{
			byte[] bits = stream.Read(2);
			var value = new Net.Rect();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 3))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.position = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 4))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.center = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 5))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.min = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 6))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.max = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[0], 7))
				value.width = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 8))
				value.height = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 1))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.size = bind.Read(stream);
			}
			if(NetConvertBase.GetBit(bits[1], 2))
				value.xMin = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 3))
				value.yMin = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 4))
				value.xMax = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 5))
				value.yMax = stream.ReadValue<Single>();
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
			NetRectBind bind = new NetRectBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Net.Rect[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new Net.Rect[count];
			if (count == 0) return value;
			NetRectBind bind = new NetRectBind();
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
			NetRectBind bind = new NetRectBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Net.Rect> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<Net.Rect>();
			if (count == 0) return value;
			NetRectBind bind = new NetRectBind();
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
