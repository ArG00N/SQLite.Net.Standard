using SQLite.Net.Interop;

namespace SQLite.Net.Platform.XamarinIOS
{
    public class SQLitePlatformIOS : ISQLitePlatform
    {
        public SQLitePlatformIOS()
        {
            SQLiteApi = new SQLiteApiIOS();
            StopwatchFactory = new StopwatchFactory();
            ReflectionService = new ReflectionService();
            VolatileService = new VolatileService();
        }

        public ISQLiteApi SQLiteApi { get; private set; }
        public IStopwatchFactory StopwatchFactory { get; private set; }
        public IReflectionService ReflectionService { get; private set; }
        public IVolatileService VolatileService { get; private set; }
    }
}