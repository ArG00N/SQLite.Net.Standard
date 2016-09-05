using System.Threading;
using SQLite.Net.Interop;
using System;

namespace SQLite.Net.Platform.Generic
{
    public class VolatileServiceGeneric : IVolatileService
    {
        public void Write(ref int transactionDepth, int depth)
        {
            throw new NotImplementedException();
            //Thread.VolatileWrite(ref transactionDepth, depth);
        }
    }
}
