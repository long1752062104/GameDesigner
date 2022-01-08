using Net.Event;
using Net.Server;
using Net.Share;
using System;

namespace Example1
{
    internal class Client:NetPlayer
    {
        [SyncVar(id = 1, hook = nameof(intCheck))]
        public int testint;
        [SyncVar(id = 2, hook = nameof(stringCheck))]
        public string teststring;

        private void intCheck(int old, int newval)
        {
            NDebug.Log(newval);
        }

        private void stringCheck(string old, string newval)
        {
            NDebug.Log(newval);
        }
    }
}