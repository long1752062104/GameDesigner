using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
	/// <summary>
	/// 这是要开发者自己写的绑定代码, 生成工具目前还没能生成这样的代码
	/// </summary>
	public struct Dictionary_String_List1__Bind : ISerialize<Dictionary<string, List<int>>>, ISerialize
	{
		public void Write(Dictionary<string, List<int>> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			foreach (var value1 in value)
			{
				stream.WriteValue(value1.Key);
				stream.WriteList(value1.Value);
			}
		}

		public Dictionary<string, List<int>> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new Dictionary<string, List<int>>();
			if (count == 0) return value;
			for (int i = 0; i < count; i++)
				value.Add(stream.ReadValue<string>(), stream.ReadList<int>());
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Dictionary<string, List<int>>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}

	/// <summary>
	/// 这是要开发者自己写的绑定代码, 生成工具目前还没能生成这样的代码
	/// </summary>
	public struct Dictionary_Int32_Boolean__Bind : ISerialize<Dictionary<int, bool>>, ISerialize
	{
		public void Write(Dictionary<int, bool> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			foreach (var value1 in value)
			{
				stream.WriteValue(value1.Key);
				stream.WriteValue(value1.Value);
			}
		}

		public Dictionary<int, bool> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new Dictionary<int, bool>();
			if (count == 0) return value;
			for (int i = 0; i < count; i++)
				value.Add(stream.ReadValue<int>(), stream.ReadValue<bool>());
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Dictionary<int, bool>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}