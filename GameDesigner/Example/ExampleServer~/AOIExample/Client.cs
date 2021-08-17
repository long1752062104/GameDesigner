using Net.Server;
using Net.AOI;
using Net;
using Net.Share;

namespace AOIExample
{
    internal class Client : NetPlayer, IGridBody
    {
        public int ID { get ; set ; }
        public Vector3 Position { get ; set ; }
        public Grid Grid { get ; set ; }

        internal ListPool<Operation> operations = new ListPool<Operation>();
        internal int getLen;

        public void OnEnter(IGridBody body)
        {
        }

        public void OnExit(IGridBody body)
        {
        }
    }
}