using System;
using System.Collections.Generic;
using Net.Serialize;
using Net.System;

namespace Binding
{
	public struct NetShareOperationListBind : ISerialize<Net.Share.OperationList>, ISerialize
	{
		public void Write(Net.Share.OperationList value, Segment stream)
		{
			int pos = stream.Position;
			stream.Position += 1;
			byte[] bits = new byte[1];
			if(value.frame != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				stream.WriteValue(value.frame);
			}
			if(value.operations != null)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				NetShareOperationArrayBind bind = new NetShareOperationArrayBind();
				bind.Write(value.operations, stream);
			}
			int pos1 = stream.Position;
			stream.Position = pos;
			stream.Write(bits, 0, 1);
			stream.Position = pos1;
		}

		public Net.Share.OperationList Read(Segment stream)
		{
			byte[] bits = stream.Read(1);
			var value = new Net.Share.OperationList();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.frame = stream.ReadValue<UInt32>();
			if(NetConvertBase.GetBit(bits[0], 2))
			{
				var bind = new NetShareOperationArrayBind();
				value.operations = bind.Read(stream);
			}
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Share.OperationList)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetShareOperationListArrayBind : ISerialize<Net.Share.OperationList[]>, ISerialize
	{
		public void Write(Net.Share.OperationList[] value, Segment stream)
		{
			int count = value.Length;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new NetShareOperationListBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public Net.Share.OperationList[] Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new Net.Share.OperationList[count];
			if (count == 0) return value;
			var bind = new NetShareOperationListBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(stream);
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((Net.Share.OperationList[])value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}

namespace Binding
{
	public struct NetShareOperationListGenericBind : ISerialize<List<Net.Share.OperationList>>, ISerialize
	{
		public void Write(List<Net.Share.OperationList> value, Segment stream)
		{
			int count = value.Count;
			stream.WriteValue(count);
			if (count == 0) return;
			var bind = new NetShareOperationListBind();
			foreach (var value1 in value)
				bind.Write(value1, stream);
		}

		public List<Net.Share.OperationList> Read(Segment stream)
		{
			var count = stream.ReadValue<int>();
			var value = new List<Net.Share.OperationList>();
			if (count == 0) return value;
			var bind = new NetShareOperationListBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(stream));
			return value;
		}

		public void WriteValue(object value, Segment stream)
		{
			Write((List<Net.Share.OperationList>)value, stream);
		}

		public object ReadValue(Segment stream)
		{
			return Read(stream);
		}
	}
}
