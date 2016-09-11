using System.Threading;
using SQLite.Net.Interop;

namespace SQLite.Net
{
    public class VolatileService : IVolatileService
    {
        public void Write(ref int transactionDepth, int depth)
        {
            Volatile.Write(ref transactionDepth, depth);
        }
    }
}