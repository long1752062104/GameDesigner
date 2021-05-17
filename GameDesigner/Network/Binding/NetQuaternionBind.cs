using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct NetQuaternionBind : ISerialize<Net.Quaternion>
	{
		public void Write(Net.Quaternion value, Segment strem)
		{
			int pos = strem.Position;
			strem.Position += 1;
			byte[] bits = new byte[1];
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
			if(value.z != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				strem.WriteValue(value.z);
			}
			if(value.w != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				strem.WriteValue(value.w);
			}
			int pos1 = strem.Position;
			strem.Position = pos;
			strem.Write(bits, 0, 1);
			strem.Position = pos1;
		}

		public Net.Quaternion Read(Segment strem)
		{
			byte[] bits = strem.Read(1);
			var value = new Net.Quaternion();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.z = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 4))
				value.w = strem.ReadValue<Single>();
			return value;
		}
	}
}

namespace Binding
{
	public struct NetQuaternionArrayBind : ISerialize<Net.Quaternion[]>
	{
		public void Write(Net.Quaternion[] value, Segment strem)
		{
			int count = value.Length;
			strem.WriteValue(count);
			if (count == 0) return;
			NetQuaternionBind bind = new NetQuaternionBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public Net.Quaternion[] Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new Net.Quaternion[count];
			if (count == 0) return value;
			NetQuaternionBind bind = new NetQuaternionBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(strem);
			return value;
		}
	}
}

namespace Binding
{
	public struct NetQuaternionGenericBind : ISerialize<List<Net.Quaternion>>
	{
		public void Write(List<Net.Quaternion> value, Segment strem)
		{
			int count = value.Count;
			strem.WriteValue(count);
			if (count == 0) return;
			NetQuaternionBind bind = new NetQuaternionBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public List<Net.Quaternion> Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new List<Net.Quaternion>();
			if (count == 0) return value;
			NetQuaternionBind bind = new NetQuaternionBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(strem));
			return value;
		}
	}
}
