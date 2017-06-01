using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SQLite.Net
{
    public class SQLiteCommandResult
    {
        public Collection<Dictionary<string, object>> Data { get; } = new Collection<Dictionary<string, object>>();
        public Collection<string> ColumnNames { get; set; } = new Collection<string>();
    }
}
