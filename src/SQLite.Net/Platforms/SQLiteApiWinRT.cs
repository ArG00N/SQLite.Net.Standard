using System;
using SQLite.Net.Interop;
using System.Runtime.InteropServices;
using Sqlite3DatabaseHandle = System.IntPtr;
using Sqlite3Statement = System.IntPtr;

namespace SQLite.Net.Platform.WinRT
{
    public class SQLiteApiWinRT : ISQLiteApiExt
    {
        public SQLiteApiWinRT(string directoryPath)
        {
            SQLite3UWP.SetDirectory(/*temp directory type*/2, directoryPath);
        }

        public int BindBlob(IDbStatement stmt, int index, byte[] val, int n, IntPtr free)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.BindBlob(dbStatement.InternalStmt, index, val, n, free);
        }

        public int BindDouble(IDbStatement stmt, int index, double val)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.BindDouble(dbStatement.InternalStmt, index, val);
        }

        public int BindInt(IDbStatement stmt, int index, int val)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.BindInt(dbStatement.InternalStmt, index, val);
        }

        public int BindInt64(IDbStatement stmt, int index, long val)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.BindInt64(dbStatement.InternalStmt, index, val);
        }

        public int BindNull(IDbStatement stmt, int index)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.BindNull(dbStatement.InternalStmt, index);
        }

        public int BindParameterIndex(IDbStatement stmt, string name)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.BindParameterIndex(dbStatement.InternalStmt, name);
        }

        public int BindText16(IDbStatement stmt, int index, string val, int n, IntPtr free)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.BindText(dbStatement.InternalStmt, index, val, n, free);
        }

        public Result BusyTimeout(IDbHandle db, int milliseconds)
        {
            var dbHandle = (DbHandle)db;
            return SQLite3UWP.BusyTimeout(dbHandle.InternalDbHandle, milliseconds);
        }

        public int Changes(IDbHandle db)
        {
            var dbHandle = (DbHandle)db;
            return SQLite3UWP.Changes(dbHandle.InternalDbHandle);
        }

        public Result Close(IDbHandle db)
        {
            var dbHandle = (DbHandle)db;
            return SQLite3UWP.Close(dbHandle.InternalDbHandle);
        }

        public Result Initialize()
        {
            throw new NotSupportedException();
        }
        public Result Shutdown()
        {
            throw new NotSupportedException();
        }

        public Result Config(ConfigOption option)
        {
            return SQLite3UWP.Config(option);
        }


        public byte[] ColumnBlob(IDbStatement stmt, int index)
        {
            var dbStatement = (DbStatement)stmt;
            var length = ColumnBytes(stmt, index);
            var result = new byte[length];
            if (length > 0)
            {
                Marshal.Copy(SQLite3UWP.ColumnBlob(dbStatement.InternalStmt, index), result, 0, length);
            }

            return result;
        }

        public byte[] ColumnByteArray(IDbStatement stmt, int index)
        {
            return ColumnBlob(stmt, index);
        }

        public int ColumnBytes(IDbStatement stmt, int index)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.ColumnBytes(dbStatement.InternalStmt, index);
        }

        public int ColumnCount(IDbStatement stmt)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.ColumnCount(dbStatement.InternalStmt);
        }

        public double ColumnDouble(IDbStatement stmt, int index)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.ColumnDouble(dbStatement.InternalStmt, index);
        }

        public int ColumnInt(IDbStatement stmt, int index)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.ColumnInt(dbStatement.InternalStmt, index);
        }

        public long ColumnInt64(IDbStatement stmt, int index)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.ColumnInt64(dbStatement.InternalStmt, index);
        }

        public string ColumnName16(IDbStatement stmt, int index)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.ColumnName16(dbStatement.InternalStmt, index);
        }

        public string ColumnText16(IDbStatement stmt, int index)
        {
            var dbStatement = (DbStatement)stmt;
            return Marshal.PtrToStringUni(SQLite3UWP.ColumnText16(dbStatement.InternalStmt, index));
        }

        public ColType ColumnType(IDbStatement stmt, int index)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.ColumnType(dbStatement.InternalStmt, index);
        }

        public int LibVersionNumber()
        {
            return SQLite3UWP.sqlite3_libversion_number();
        }

        public string SourceID()
        {
            return Marshal.PtrToStringAnsi(SQLite3UWP.sqlite3_sourceid());  
        }        

        public Result EnableLoadExtension(IDbHandle db, int onoff)
        {
            return (Result)1;
        }

        public string Errmsg16(IDbHandle db)
        {
            var dbHandle = (DbHandle)db;
            return SQLite3UWP.GetErrmsg(dbHandle.InternalDbHandle);
        }

        public Result Finalize(IDbStatement stmt)
        {
            var dbStatement = (DbStatement)stmt;
            var internalStmt = dbStatement.InternalStmt;
            return SQLite3UWP.Finalize(internalStmt);
        }

        public long LastInsertRowid(IDbHandle db)
        {
            var dbHandle = (DbHandle)db;
            return SQLite3UWP.LastInsertRowid(dbHandle.InternalDbHandle);
        }

        public Result Open(byte[] filename, out IDbHandle db, int flags, IntPtr zvfs)
        {
            var ret = SQLite3UWP.Open(filename, out Sqlite3DatabaseHandle internalDbHandle, flags, zvfs);
            db = new DbHandle(internalDbHandle);
            return ret;
        }

        public ExtendedResult ExtendedErrCode(IDbHandle db)
        {
            var dbHandle = (DbHandle)db;
            return SQLite3UWP.sqlite3_extended_errcode(dbHandle.InternalDbHandle);
        }

        public IDbStatement Prepare2(IDbHandle db, string query)
        {
            if (query == null)
            {
                throw new ArgumentException(nameof(query));
            }

            var dbHandle = (DbHandle)db;
            var r = SQLite3UWP.Prepare2(dbHandle.InternalDbHandle, query, query.Length, out Sqlite3DatabaseHandle stmt, IntPtr.Zero);
            if (r != Result.OK)
            {
                throw SQLiteException.New(r, SQLite3UWP.GetErrmsg(dbHandle.InternalDbHandle));
            }
            return new DbStatement(stmt);
        }

        public Result Reset(IDbStatement stmt)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.Reset(dbStatement.InternalStmt);
        }

        public Result Step(IDbStatement stmt)
        {
            var dbStatement = (DbStatement)stmt;
            return SQLite3UWP.Step(dbStatement.InternalStmt);
        }

        #region Backup

        public IDbBackupHandle BackupInit(IDbHandle destHandle, string destName, IDbHandle srcHandle, string srcName)
        {
            var internalDestDb = (DbHandle)destHandle;
            var internalSrcDb = (DbHandle)srcHandle;

            var p = SQLite3UWP.sqlite3_backup_init(internalDestDb.InternalDbHandle,
                                                                  destName,
                                                                  internalSrcDb.InternalDbHandle,
                                                                  srcName);

            if (p == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return new DbBackupHandle(p);
            }
        }

        public Result BackupStep(IDbBackupHandle handle, int pageCount)
        {
            var internalBackup = (DbBackupHandle)handle;
            return SQLite3UWP.sqlite3_backup_step(internalBackup.DbBackupPtr, pageCount);
        }

        public Result BackupFinish(IDbBackupHandle handle)
        {
            var internalBackup = (DbBackupHandle)handle;
            return SQLite3UWP.sqlite3_backup_finish(internalBackup.DbBackupPtr);
        }

        public int BackupRemaining(IDbBackupHandle handle)
        {
            var internalBackup = (DbBackupHandle)handle;
            return SQLite3UWP.sqlite3_backup_remaining(internalBackup.DbBackupPtr);
        }

        public int BackupPagecount(IDbBackupHandle handle)
        {
            var internalBackup = (DbBackupHandle)handle;
            return SQLite3UWP.sqlite3_backup_pagecount(internalBackup.DbBackupPtr);
        }

        public int Sleep(int millis)
        {
            return SQLite3UWP.sqlite3_sleep(millis);
        }

        private struct DbBackupHandle : IDbBackupHandle
        {
            public DbBackupHandle(IntPtr dbBackupPtr)
                : this()
            {
                DbBackupPtr = dbBackupPtr;
            }

            internal IntPtr DbBackupPtr { get; set; }

        }

        #endregion

        private struct DbHandle : IDbHandle
        {
            public DbHandle(Sqlite3DatabaseHandle internalDbHandle)
                : this()
            {
                InternalDbHandle = internalDbHandle;
            }

            public Sqlite3DatabaseHandle InternalDbHandle { get; set; }

        }

        private struct DbStatement : IDbStatement
        {
            public DbStatement(Sqlite3Statement internalStmt)
                : this()
            {
                InternalStmt = internalStmt;
            }

            internal Sqlite3Statement InternalStmt { get; set; }

        }
    }
}