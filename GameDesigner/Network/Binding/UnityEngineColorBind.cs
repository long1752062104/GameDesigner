using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct UnityEngineColorBind : ISerialize<UnityEngine.Color>
	{
		public void Write(UnityEngine.Color value, Segment strem)
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

		public UnityEngine.Color Read(Segment strem)
		{
			byte[] bits = strem.Read(1);
			var value = new UnityEngine.Color();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.r = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.g = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.b = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 4))
				value.a = strem.ReadValue<Single>();
			return value;
		}
	}
}

namespace Binding
{
	public struct UnityEngineColorArrayBind : ISerialize<UnityEngine.Color[]>
	{
		public void Write(UnityEngine.Color[] value, Segment strem)
		{
			int count = value.Length;
			strem.WriteValue(count);
			if (count == 0) return;
			UnityEngineColorBind bind = new UnityEngineColorBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public UnityEngine.Color[] Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new UnityEngine.Color[count];
			if (count == 0) return value;
			UnityEngineColorBind bind = new UnityEngineColorBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(strem);
			return value;
		}
	}
}

namespace Binding
{
	public struct UnityEngineColorGenericBind : ISerialize<List<UnityEngine.Color>>
	{
		public void Write(List<UnityEngine.Color> value, Segment strem)
		{
			int count = value.Count;
			strem.WriteValue(count);
			if (count == 0) return;
			UnityEngineColorBind bind = new UnityEngineColorBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public List<UnityEngine.Color> Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new List<UnityEngine.Color>();
			if (count == 0) return value;
			UnityEngineColorBind bind = new UnityEngineColorBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(strem));
			return value;
		}
	}
}
