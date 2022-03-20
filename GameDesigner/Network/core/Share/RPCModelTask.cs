namespace Net.Share
{
    public class RPCModelTask
    {
        public bool IsCompleted { get; internal set; }
        public RPCModel model;
        internal int referenceCount;
        internal bool intercept;
    }
}
