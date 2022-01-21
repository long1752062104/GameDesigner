using System;
using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
	public struct UnityEngineVector2IntBind : ISerialize<UnityEngine.Vector2Int>, ISerialize
	{
		public void Write(UnityEngine.Vector2Int value, Segment stream)
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
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 1);
			stream.Position = pos1;
		}

		public UnityEngine.Vector2Int Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new UnityEngine.Vector2Int();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.x = stream.ReadValue<Int32>();
			if(NetConvertBase.GetBit(bits[0], 2))
				value.y = stream.ReadValue<Int32>();
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.Vector2Int)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct UnityEngineVector2IntArrayBind : ISerialize<UnityEngine.Vector2Int[]>, ISerialize
	{
		public void Write(UnityEngine.Vector2Int[] value, Segment stream)
		{
			int count = value.Length;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new UnityEngineVector2IntBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public UnityEngine.Vector2Int[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new UnityEngine.Vector2Int[count];
			if (count == 0) return value;
			var bind = new UnityEngineVector2IntBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((UnityEngine.Vector2Int[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct UnityEngineVector2IntGenericBind : ISerialize<List<UnityEngine.Vector2Int>>, ISerialize
	{
		public void Write(List<UnityEngine.Vector2Int> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new UnityEngineVector2IntBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<UnityEngine.Vector2Int> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<UnityEngine.Vector2Int>();
			if (count == 0) return value;
			var bind = new UnityEngineVector2IntBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<UnityEngine.Vector2Int>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
