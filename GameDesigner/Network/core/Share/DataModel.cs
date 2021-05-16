namespace Net.Share
{
    using System.Collections.Generic;

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class DataModel
    {
        public List<RPCModel> buffers = new List<RPCModel>();
        public int count;
    }
}
