using SQLite.Net.Interop;

namespace SQLite.Net.Platform.OSX
{
    public class SQLitePlatformOSX : ISQLitePlatform
    {
        public SQLitePlatformOSX()
        {
            SQLiteApi = new SQLiteApiOSX();
            StopwatchFactory = new StopwatchFactory();
            ReflectionService = new ReflectionService();
            //TODO: The original VolatileService is slightly different on OSX. There's no indication as to whether this was done for compilation reasons, or OSX just behaves differently. So, in future we may need to dependency inject the VolatileService for OSX. If that happens, the VolatileService is available in the original SQLite PCL repo.
            VolatileService = new VolatileService();
        }

        public ISQLiteApi SQLiteApi { get; private set; }
        public IStopwatchFactory StopwatchFactory { get; private set; }
        public IReflectionService ReflectionService { get; private set; }
        public IVolatileService VolatileService { get; private set; }
    }
}
