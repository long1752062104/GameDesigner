using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct UnityEngineColor32Bind : ISerialize<UnityEngine.Color32>
	{
		public void Write(UnityEngine.Color32 value, Segment strem)
		{
			int pos = strem.Position;
			strem.Position += 1;
			byte[] bits = new byte[1];
			if(value.r != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				strem.WriteValue(value.r);
			}
			if(value.g != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				strem.WriteValue(value.g);
			}
			if(value.b != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				strem.WriteValue(value.b);
			}
			if(value.a != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				strem.WriteValue(value.a);
			}
			int pos1 = strem.Position;
			strem.Position = pos;
			strem.Write(bits, 0, 1);
			strem.Position = pos1;
		}

		public UnityEngine.Color32 Read(Segment strem)
		{
			byte[] bits = strem.Read(1);
			var value = new UnityEngine.Color32();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.r = strem.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.g = strem.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.b = strem.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 4))
				value.a = strem.ReadValue<Byte>();
			return value;
		}
	}
}

namespace Binding
{
	public struct UnityEngineColor32ArrayBind : ISerialize<UnityEngine.Color32[]>
	{
		public void Write(UnityEngine.Color32[] value, Segment strem)
		{
			int count = value.Length;
			strem.WriteValue(count);
			if (count == 0) return;
			UnityEngineColor32Bind bind = new UnityEngineColor32Bind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public UnityEngine.Color32[] Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new UnityEngine.Color32[count];
			if (count == 0) return value;
			UnityEngineColor32Bind bind = new UnityEngineColor32Bind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(strem);
			return value;
		}
	}
}

namespace Binding
{
	public struct UnityEngineColor32GenericBind : ISerialize<List<UnityEngine.Color32>>
	{
		public void Write(List<UnityEngine.Color32> value, Segment strem)
		{
			int count = value.Count;
			strem.WriteValue(count);
			if (count == 0) return;
			UnityEngineColor32Bind bind = new UnityEngineColor32Bind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public List<UnityEngine.Color32> Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new List<UnityEngine.Color32>();
			if (count == 0) return value;
			UnityEngineColor32Bind bind = new UnityEngineColor32Bind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(strem));
			return value;
		}
	}
}
