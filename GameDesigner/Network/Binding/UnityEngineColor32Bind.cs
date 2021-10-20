using System;
using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
	public struct UnityEngineColor32Bind : ISerialize<UnityEngine.Color32>, ISerialize
	{
		public void Write(UnityEngine.Color32 value, Segment stream)
		{
			int pos = stream.Position;
			stream.Position += 1;
			byte[] bits = new byte[1];
			if(value.r != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				stream.WriteValue(value.r);
			}
			if(value.g != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				stream.WriteValue(value.g);
			}
			if(value.b != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 3, true);
				stream.WriteValue(value.b);
			}
			if(value.a != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 4, true);
				stream.WriteValue(value.a);
			}
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 1);
			stream.Position = pos1;
		}

		public UnityEngine.Color32 Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new UnityEngine.Color32();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.r = stream.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.g = stream.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 3))
				value.b = stream.ReadValue<Byte>();
			if(NetConvertBase.GetBit(bits[0], 4))
				value.a = stream.ReadValue<Byte>();
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.Color32)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct UnityEngineColor32ArrayBind : ISerialize<UnityEngine.Color32[]>, ISerialize
	{
		public void Write(UnityEngine.Color32[] value, Segment stream)
		{
			int count = value.Length;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new UnityEngineColor32Bind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public UnityEngine.Color32[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new UnityEngine.Color32[count];
			if (count == 0) return value;
			var bind = new UnityEngineColor32Bind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.Color32[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct UnityEngineColor32GenericBind : ISerialize<List<UnityEngine.Color32>>, ISerialize
	{
		public void Write(List<UnityEngine.Color32> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new UnityEngineColor32Bind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<UnityEngine.Color32> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<UnityEngine.Color32>();
			if (count == 0) return value;
			var bind = new UnityEngineColor32Bind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<UnityEngine.Color32>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
