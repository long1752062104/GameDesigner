using System;
using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
	public struct UnityEngineQuaternionBind : ISerialize<UnityEngine.Quaternion>, ISerialize
	{
		public void Write(UnityEngine.Quaternion value, Segment stream)
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
			if(value.w != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				stream.WriteValue(value.w);
			}
			if(value.eulerAngles != default)
			{
				NetConvertBase.SetBit(ref bits[0], 5, true);
				var bind = new UnityEngineVector3Bind();
				bind.Write(value.eulerAngles, stream);
			}
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 1);
			stream.Position = pos1;
		}

		public UnityEngine.Quaternion Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new UnityEngine.Quaternion();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.z = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 4))
				value.w = stream.ReadValue<Single>();
			if(NetConvertBase.GetBit(bits[0], 5))
			{
				var bind = new UnityEngineVector3Bind();
				value.eulerAngles = bind.Read(stream);
			}
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.Quaternion)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct UnityEngineQuaternionArrayBind : ISerialize<UnityEngine.Quaternion[]>, ISerialize
	{
		public void Write(UnityEngine.Quaternion[] value, Segment stream)
		{
			int count = value.Length;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new UnityEngineQuaternionBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public UnityEngine.Quaternion[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new UnityEngine.Quaternion[count];
			if (count == 0) return value;
			var bind = new UnityEngineQuaternionBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.Quaternion[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct UnityEngineQuaternionGenericBind : ISerialize<List<UnityEngine.Quaternion>>, ISerialize
	{
		public void Write(List<UnityEngine.Quaternion> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new UnityEngineQuaternionBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<UnityEngine.Quaternion> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<UnityEngine.Quaternion>();
			if (count == 0) return value;
			var bind = new UnityEngineQuaternionBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<UnityEngine.Quaternion>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
