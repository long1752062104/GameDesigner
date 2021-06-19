using System;
using System.Collections.Generic;
using Net.Share;

namespace Binding
{
	public struct NetShareOperationListBind : ISerialize<Net.Share.OperationList>
	{
		public void Write(Net.Share.OperationList value, Segment strem)
		{
			int pos = strem.Position;
			strem.Position += 1;
			byte[] bits = new byte[1];
			if(value.frame != 0)
			{
				NetConvertBase.SetBit(ref bits[0], 1, true);
				strem.WriteValue(value.frame);
			}
			if(value.operations != null)
			{
				NetConvertBase.SetBit(ref bits[0], 2, true);
				int count = value.operations.Length;
				strem.WriteValue(count);
				if (count == 0) goto JMP;
				NetShareOperationBind bind = new NetShareOperationBind();
				foreach (var value1 in value.operations)
					bind.Write(value1, strem);
				JMP:;
			}
			int pos1 = strem.Position;
			strem.Position = pos;
			strem.Write(bits, 0, 1);
			strem.Position = pos1;
		}

		public Net.Share.OperationList Read(Segment strem)
		{
			byte[] bits = strem.Read(1);
			var value = new Net.Share.OperationList();
			if(NetConvertBase.GetBit(bits[0], 1))
				value.frame = strem.ReadValue<UInt32>();
			if(NetConvertBase.GetBit(bits[0], 2))
			{
				var count = strem.ReadValue<int>();
				value.operations = new Net.Share.Operation[count];
				if (count == 0) goto JMP;
				NetShareOperationBind bind = new NetShareOperationBind();
				for (int i = 0; i < count; i++)
					value.operations[i] = bind.Read(strem);
				JMP:;
			}
			return value;
		}
	}
}

namespace Binding
{
	public struct NetShareOperationListArrayBind : ISerialize<Net.Share.OperationList[]>
	{
		public void Write(Net.Share.OperationList[] value, Segment strem)
		{
			int count = value.Length;
			strem.WriteValue(count);
			if (count == 0) return;
			NetShareOperationListBind bind = new NetShareOperationListBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public Net.Share.OperationList[] Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new Net.Share.OperationList[count];
			if (count == 0) return value;
			NetShareOperationListBind bind = new NetShareOperationListBind();
			for (int i = 0; i < count; i++)
				value[i] = bind.Read(strem);
			return value;
		}
	}
}

namespace Binding
{
	public struct NetShareOperationListGenericBind : ISerialize<List<Net.Share.OperationList>>
	{
		public void Write(List<Net.Share.OperationList> value, Segment strem)
		{
			int count = value.Count;
			strem.WriteValue(count);
			if (count == 0) return;
			NetShareOperationListBind bind = new NetShareOperationListBind();
			foreach (var value1 in value)
				bind.Write(value1, strem);
		}

		public List<Net.Share.OperationList> Read(Segment strem)
		{
			var count = strem.ReadValue<int>();
			var value = new List<Net.Share.OperationList>();
			if (count == 0) return value;
			NetShareOperationListBind bind = new NetShareOperationListBind();
			for (int i = 0; i < count; i++)
				value.Add(bind.Read(strem));
			return value;
		}
	}
}
