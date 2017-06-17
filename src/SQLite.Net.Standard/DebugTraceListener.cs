using System.Diagnostics;

namespace SQLite.Net
{
    public sealed class DebugTraceListener : ITraceListener
    {
        private DebugTraceListener()
        {
        }

        public void Receive(string message)
        {
            Debug.WriteLine(message);
        }
    }
}