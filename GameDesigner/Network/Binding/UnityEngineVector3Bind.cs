using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct UnityEngineVector3Bind : ISerialize<UnityEngine.Vector3>
	{
		public void Write(UnityEngine.Vector3 value, Segment strem)
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
			int pos1 = strem.Position;
			strem.Position = pos;
			strem.Write(bits, 0, 1);
			strem.Position = pos1;
		}

		public UnityEngine.Vector3 Read(Segment strem)
		{
			byte[] bits = strem.Read(1);
			var value = new UnityEngine.Vector3();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = strem.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.z = strem.ReadValue<Single>();
			return value;
		}
	}
}

namespace Binding
{
	public struct UnityEngineVector3ArrayBind : ISerialize<UnityEngine.Vector3[]>
	{
		public void Write(UnityEngine.Vector3[] value, Segment strem)
		{
			int count = value.Length;
			strem.WriteValue(count);
			if (count == 0) return;
			UnityEngineVector3Bind bind = new UnityEngineVector3Bind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public UnityEngine.Vector3[] Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new UnityEngine.Vector3[count];
			if (count == 0) return value;
			UnityEngineVector3Bind bind = new UnityEngineVector3Bind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(strem);
			return value;
		}
	}
}

namespace Binding
{
	public struct UnityEngineVector3GenericBind : ISerialize<List<UnityEngine.Vector3>>
	{
		public void Write(List<UnityEngine.Vector3> value, Segment strem)
		{
			int count = value.Count;
			strem.WriteValue(count);
			if (count == 0) return;
			UnityEngineVector3Bind bind = new UnityEngineVector3Bind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public List<UnityEngine.Vector3> Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new List<UnityEngine.Vector3>();
			if (count == 0) return value;
			UnityEngineVector3Bind bind = new UnityEngineVector3Bind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(strem));
			return value;
		}
	}
}
