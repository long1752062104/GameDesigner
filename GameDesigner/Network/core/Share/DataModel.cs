namespace Net.Share
{
    using global::System.Collections.Generic;

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class DataModel
    {
        public List<RPCModel> buffers = new List<RPCModel>();
        public int count;
    }
}
