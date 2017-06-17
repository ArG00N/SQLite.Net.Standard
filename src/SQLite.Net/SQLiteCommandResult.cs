using System.Collections.ObjectModel;

namespace SQLite.Net
{
    public class SQLiteCommandResult
    {
        public SQLiteDataTable Data { get; } = new SQLiteDataTable();
        public Collection<string> ColumnNames { get; } = new Collection<string>();
    }
}
