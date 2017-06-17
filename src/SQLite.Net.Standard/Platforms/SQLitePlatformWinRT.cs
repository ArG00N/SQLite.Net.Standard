using SQLite.Net.Interop;

namespace SQLite.Net.Platform.WinRT
{
    public class SQLitePlatformWinRT : ISQLitePlatform
    {
        public SQLitePlatformWinRT(string directoryPath)
        {
            SQLiteApi = new SQLiteApiWinRT(directoryPath);
            VolatileService = new VolatileService();
            StopwatchFactory = new StopwatchFactory();
            ReflectionService = new ReflectionService();
        }

        public string DatabaseRootDirectory { get; set; }

        public ISQLiteApi SQLiteApi { get; private set; }
        public IStopwatchFactory StopwatchFactory { get; private set; }
        public IReflectionService ReflectionService { get; private set; }
        public IVolatileService VolatileService { get; private set; }
    }
}