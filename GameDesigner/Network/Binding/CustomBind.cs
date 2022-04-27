using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
	/// <summary>
	/// 字典绑定
	/// </summary>
	public struct DictionaryBind<TKey, TValue>
	{
		public void Write(Dictionary<TKey, TValue> value, Segment stream, ISerialize<TValue> bind)
		{
			int count = value.Count;
			stream.Write(count);
			if (count == 0) return;
			foreach (var value1 in value)
			{
				stream.WriteValue(value1.Key);
				bind.Write(value1.Value, stream);
			}
		}

		public Dictionary<TKey, TValue> Read(Segment stream, ISerialize<TValue> bind)
		{
			var count = stream.ReadInt32();
			var value = new Dictionary<TKey, TValue>();
			if (count == 0) return value;
			for (int i = 0; i < count; i++)
			{
				var key = stream.ReadValue<TKey>();
				var tvalue = bind.Read(stream);
				value.Add(key, tvalue);
			}
			return value;
		}
	}
}