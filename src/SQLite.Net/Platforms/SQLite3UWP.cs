using System;
using System.Runtime.InteropServices;
using SQLite.Net.Interop;

namespace SQLite.Net.Platform.WinRT
{
    internal static class SQLite3UWP
    {
        [DllImport("sqlite3", EntryPoint = "sqlite3_open", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Open([MarshalAs(UnmanagedType.LPStr)] string filename, out IntPtr db, int flags, IntPtr zvfs);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Open(byte[] filename, out IntPtr db, int flags, IntPtr zvfs);

        [DllImport("sqlite3", EntryPoint = "sqlite3_open16", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Open16([MarshalAs(UnmanagedType.LPWStr)] string filename, out IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_close", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Close(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_config", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Config(ConfigOption option);

        [DllImport("sqlite3", EntryPoint = "sqlite3_win32_set_directory", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern int SetDirectory(uint directoryType, string directoryPath);

        [DllImport("sqlite3", EntryPoint = "sqlite3_busy_timeout", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result BusyTimeout(IntPtr db, int milliseconds);

        [DllImport("sqlite3", EntryPoint = "sqlite3_changes", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Changes(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Prepare2(IntPtr db, [MarshalAs(UnmanagedType.LPStr)] string sql, int numBytes, out IntPtr stmt, IntPtr pzTail);

        [DllImport("sqlite3", EntryPoint = "sqlite3_step", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Step(IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_reset", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Reset(IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_finalize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result Finalize(IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_last_insert_rowid", CallingConvention = CallingConvention.Cdecl)]
        internal static extern long LastInsertRowid(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg16", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Errmsg(IntPtr db);

        internal static string GetErrmsg(IntPtr db)
        {
            return Marshal.PtrToStringUni(Errmsg(db));
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_index", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BindParameterIndex(IntPtr stmt, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_null", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BindNull(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BindInt(IntPtr stmt, int index, int val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int64", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BindInt64(IntPtr stmt, int index, long val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_double", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BindDouble(IntPtr stmt, int index, double val);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_text16", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        internal static extern int BindText(IntPtr stmt, int index, [MarshalAs(UnmanagedType.LPWStr)] string val, int n, IntPtr free);

        [DllImport("sqlite3", EntryPoint = "sqlite3_bind_blob", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_count", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ColumnCount(IntPtr stmt);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr ColumnName(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_name16", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ColumnName16Internal(IntPtr stmt, int index);
        internal static string ColumnName16(IntPtr stmt, int index)
        {
            return Marshal.PtrToStringUni(ColumnName16Internal(stmt, index));
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_type", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ColType ColumnType(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ColumnInt(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_int64", CallingConvention = CallingConvention.Cdecl)]
        internal static extern long ColumnInt64(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_double", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double ColumnDouble(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_text", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr ColumnText(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_text16", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr ColumnText16(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr ColumnBlob(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ColumnBytes(IntPtr stmt, int index);

        [DllImport("sqlite3", EntryPoint = "sqlite3_extended_errcode", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ExtendedResult sqlite3_extended_errcode(IntPtr db);

        [DllImport("sqlite3", EntryPoint = "sqlite3_libversion_number", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_libversion_number();

        [DllImport("sqlite3", EntryPoint = "sqlite3_sourceid", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_sourceid();

        #region Backup

        [DllImport("sqlite3", EntryPoint = "sqlite3_backup_init", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr sqlite3_backup_init(IntPtr destDB,
            [MarshalAs(UnmanagedType.LPStr)] string destName,
            IntPtr srcDB,
            [MarshalAs(UnmanagedType.LPStr)] string srcName);

        [DllImport("sqlite3", EntryPoint = "sqlite3_backup_step", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result sqlite3_backup_step(IntPtr backup, int pageCount);

        [DllImport("sqlite3", EntryPoint = "sqlite3_backup_finish", CallingConvention = CallingConvention.Cdecl)]
        internal static extern Result sqlite3_backup_finish(IntPtr backup);
        
        [DllImport("sqlite3", EntryPoint = "sqlite3_backup_remaining", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_backup_remaining(IntPtr backup);

        [DllImport("sqlite3", EntryPoint = "sqlite3_backup_pagecount", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_backup_pagecount(IntPtr backup);

        [DllImport("sqlite3", EntryPoint = "sqlite3_sleep", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sqlite3_sleep(int millis);

        #endregion
    }
}