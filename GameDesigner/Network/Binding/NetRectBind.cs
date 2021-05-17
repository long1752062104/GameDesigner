using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct NetRectBind : ISerialize<Net.Rect>
	{
		public void Write(Net.Rect value, Segment strem)
		{
			int pos = strem.Position;
			strem.Position += 2;
			byte[] bits = new byte[2];
			if(value.x != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				strem.WriteValue(value.x);
			}
			if(value.y != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				strem.WriteValue(value.y);
			}
			if(value.position != null)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.position, strem);
			}
			if(value.center != null)
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.center, strem);
			}
			if(value.min != null)
			{
				NetConvertBase.SetBit(ref bits[0], 5, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.min, strem);
			}
			if(value.max != null)
			{
				NetConvertBase.SetBit(ref bits[0], 6, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.max, strem);
			}
			if(value.width != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 7, true);
				strem.WriteValue(value.width);
			}
			if(value.height != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 8, true);
				strem.WriteValue(value.height);
			}
			if(value.size != null)
			{
				NetConvertBase.SetBit(ref bits[1], 1, true);
				NetVector2Bind bind = new NetVector2Bind();
				bind.Write(value.size, strem);
			}
			if(value.xMin != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 2, true);
				strem.WriteValue(value.xMin);
			}
			if(value.yMin != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 3, true);
				strem.WriteValue(value.yMin);
			}
			if(value.xMax != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 4, true);
				strem.WriteValue(value.xMax);
			}
			if(value.yMax != 0)
			{
				NetConvertBase.SetBit(ref bits[1], 5, true);
				strem.WriteValue(value.yMax);
			}
			int pos1 = strem.Position;
			strem.Position = pos;
			strem.Write(bits, 0, 2);
			strem.Position = pos1;
		}

		public Net.Rect Read(Segment strem)
		{
			byte[] bits = strem.Read(2);
			var value = new Net.Rect();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 3))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.position = bind.Read(strem);
			}
			if(NetConvertBase.GetBit(bits[0], 4))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.center = bind.Read(strem);
			}
			if(NetConvertBase.GetBit(bits[0], 5))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.min = bind.Read(strem);
			}
			if(NetConvertBase.GetBit(bits[0], 6))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.max = bind.Read(strem);
			}
			if(NetConvertBase.GetBit(bits[0], 7))
				value.width = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 8))
				value.height = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 1))
			{
				NetVector2Bind bind = new NetVector2Bind();
				value.size = bind.Read(strem);
			}
			if(NetConvertBase.GetBit(bits[1], 2))
				value.xMin = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 3))
				value.yMin = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 4))
				value.xMax = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[1], 5))
				value.yMax = strem.ReadValue<Single>();
			return value;
		}
	}
}

namespace Binding
{
	public struct NetRectArrayBind : ISerialize<Net.Rect[]>
	{
		public void Write(Net.Rect[] value, Segment strem)
		{
			int count = value.Length;
			strem.WriteValue(count);
			if (count == 0) return;
			NetRectBind bind = new NetRectBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public Net.Rect[] Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new Net.Rect[count];
			if (count == 0) return value;
			NetRectBind bind = new NetRectBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(strem);
			return value;
		}
	}
}

namespace Binding
{
	public struct NetRectGenericBind : ISerialize<List<Net.Rect>>
	{
		public void Write(List<Net.Rect> value, Segment strem)
		{
			int count = value.Count;
			strem.WriteValue(count);
			if (count == 0) return;
			NetRectBind bind = new NetRectBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public List<Net.Rect> Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new List<Net.Rect>();
			if (count == 0) return value;
			NetRectBind bind = new NetRectBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(strem));
			return value;
		}
	}
}
