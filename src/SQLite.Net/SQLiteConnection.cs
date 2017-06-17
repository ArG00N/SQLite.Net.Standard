//
// Copyright (c) 2012 Krueger Systems, Inc.
// Copyright (c) 2013 Øystein Krog (oystein.krog@gmail.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SQLite.Net.Interop;

namespace SQLite.Net
{
    /// <summary>
    ///     Represents an open connection to a SQLite database.
    /// </summary>

    public class SQLiteConnection : IDisposable
    {
        internal static readonly IDbHandle NullHandle = default(IDbHandle);

        /// <summary>
        ///     Used to list some code that we want the MonoTouch linker
        ///     to see, but that we never want to actually execute.
        /// </summary>
        private readonly Random _rand = new Random();

        private readonly object _tableMappingsLocks;
        private TimeSpan _busyTimeout;
        private long _elapsedMilliseconds;

        private bool _open;
        private IStopwatch _sw;
        private int _transactionDepth;


        /// <summary>
        ///     Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
        /// </summary>
        /// <param name="sqlitePlatform"></param>
        /// <param name="databasePath">
        ///     Specifies the path to the database file.
        /// </param>
        /// <param name="storeDateTimeAsTicks">
        ///     Specifies whether to store DateTime properties as ticks (true) or strings (false). You
        ///     absolutely do want to store them as Ticks in all new projects. The option to set false is
        ///     only here for backwards compatibility. There is a *significant* speed advantage, with no
        ///     down sides, when setting storeDateTimeAsTicks = true.
        /// </param>
        /// <param name="serializer">
        ///     Blob serializer to use for storing undefined and complex data structures. If left null
        ///     these types will thrown an exception as usual.
        /// </param>
        /// <param name="extraTypeMappings">
        ///     Any extra type mappings that you wish to use for overriding the default for creating
        ///     column definitions for SQLite DDL in the class Orm (snake in Swedish).
        /// </param>
        /// <param name="resolver">
        ///     A contract resovler for resolving interfaces to concreate types during object creation
        /// </param>
        /// 

        public SQLiteConnection(ISQLitePlatform sqlitePlatform, string databasePath, bool storeDateTimeAsTicks = true, IBlobSerializer serializer = null, IDictionary<Type, string> extraTypeMappings = null, IContractResolver resolver = null) : this(sqlitePlatform, databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, storeDateTimeAsTicks, serializer, extraTypeMappings, resolver)
        {
        }

        /// <summary>
        ///     Constructs a new SQLiteConnection and opens a SQLite database specified by databasePath.
        /// </summary>
        /// <param name="sqlitePlatform"></param>
        /// <param name="databasePath">
        ///     Specifies the path to the database file.
        /// </param>
        /// <param name="openFlags"></param>
        /// <param name="storeDateTimeAsTicks">
        ///     Specifies whether to store DateTime properties as ticks (true) or strings (false). You
        ///     absolutely do want to store them as Ticks in all new projects. The option to set false is
        ///     only here for backwards compatibility. There is a *significant* speed advantage, with no
        ///     down sides, when setting storeDateTimeAsTicks = true.
        /// </param>
        /// <param name="serializer">
        ///     Blob serializer to use for storing undefined and complex data structures. If left null
        ///     these types will thrown an exception as usual.
        /// </param>
        /// <param name="extraTypeMappings">
        ///     Any extra type mappings that you wish to use for overriding the default for creating
        ///     column definitions for SQLite DDL in the class Orm (snake in Swedish).
        /// </param>
        /// <param name="resolver">
        ///     A contract resovler for resolving interfaces to concreate types during object creation
        /// </param>
        /// 

        public SQLiteConnection( ISQLitePlatform sqlitePlatform, string databasePath, SQLiteOpenFlags openFlags,
            bool storeDateTimeAsTicks = true, IBlobSerializer serializer = null, IDictionary<Type, string> extraTypeMappings = null,
            IContractResolver resolver = null)
        {
            if (sqlitePlatform == null)
            {
                throw new ArgumentNullException(nameof(sqlitePlatform));
            }
            ExtraTypeMappings = extraTypeMappings ?? new Dictionary<Type, string>();
            Serializer = serializer;
            Platform = sqlitePlatform;
            Resolver = resolver ?? ContractResolver.Current;

            _tableMappingsLocks = new object();

            if (string.IsNullOrEmpty(databasePath))
            {
                throw new ArgumentException("Must be specified", nameof(databasePath));
            }

            DatabasePath = databasePath;

            IDbHandle handle;
            var databasePathAsBytes = GetNullTerminatedUtf8(DatabasePath);
            var r = Platform.SQLiteApi.Open(databasePathAsBytes, out handle, (int)openFlags, IntPtr.Zero);

            Handle = handle;
            if (r != Result.OK)
            {
                throw SQLiteException.New(r, string.Format("Could not open database file: {0} ({1})", DatabasePath, r));
            }

            if (handle == null)
            {
                throw new NullReferenceException("Database handle is null");
            }

            Handle = handle;

            _open = true;

            StoreDateTimeAsTicks = storeDateTimeAsTicks;

            BusyTimeout = TimeSpan.FromSeconds(0.1);
        }


        public IBlobSerializer Serializer { get; private set; }


        public IDbHandle Handle { get; private set; }

        public string DatabasePath { get; private set; }


        public bool TimeExecution { get; set; }



        public ITraceListener TraceListener { get; set; }


        public bool StoreDateTimeAsTicks { get; private set; }


        public IDictionary<Type, string> ExtraTypeMappings { get; private set; }


        public IContractResolver Resolver { get; private set; }

        /// <summary>
        ///     Sets a busy handler to sleep the specified amount of time when a table is locked.
        ///     The handler will sleep multiple times until a total time of <see cref="BusyTimeout" /> has accumulated.
        /// </summary>

        public TimeSpan BusyTimeout
        {
            get { return _busyTimeout; }
            set
            {
                _busyTimeout = value;
                if (Handle != NullHandle)
                {
                    Platform.SQLiteApi.BusyTimeout(Handle, (int)_busyTimeout.TotalMilliseconds);
                }
            }
        }

        /// <summary>
        ///     Whether <see cref="BeginTransaction" /> has been called and the database is waiting for a <see cref="Commit" />.
        /// </summary>

        public bool IsInTransaction
        {
            get { return _transactionDepth > 0; }
        }


        public ISQLitePlatform Platform { get; private set; }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public void EnableLoadExtension(int onoff)
        {
            var r = Platform.SQLiteApi.EnableLoadExtension(Handle, onoff);
            if (r != Result.OK)
            {
                var msg = Platform.SQLiteApi.Errmsg16(Handle);
                throw SQLiteException.New(r, msg);
            }
        }

        private static byte[] GetNullTerminatedUtf8(string s)
        {
            var utf8Length = Encoding.UTF8.GetByteCount(s);
            var bytes = new byte[utf8Length + 1];
            Encoding.UTF8.GetBytes(s, 0, s.Length, bytes, 0);
            return bytes;
        }


        /// <summary>
        ///     Creates a new SQLiteCommand. Can be overridden to provide a sub-class.
        /// </summary>
        /// <seealso cref="SQLiteCommand.OnInstanceCreated" />

        protected SQLiteCommand NewCommand()
        {
            return new SQLiteCommand(Platform, this);
        }

        /// <summary>
        ///     Creates a new SQLiteCommand given the command text with arguments. Place a '?'
        ///     in the command text for each of the arguments.
        /// </summary>
        /// <param name="cmdText">
        ///     The fully escaped SQL.
        /// </param>
        /// <param name="args">
        ///     Arguments to substitute for the occurences of '?' in the command text.
        /// </param>
        /// <returns>
        ///     A <see cref="SQLiteCommand" />
        /// </returns>

        public SQLiteCommand CreateCommand(string cmdText, params object[] args)
        {
            if (!_open)
            {
                throw SQLiteException.New(Result.Error, "Cannot create commands from unopened database");
            }

            var cmd = NewCommand();
            cmd.CommandText = cmdText;
            foreach (var o in args)
            {
                cmd.Bind(o);
            }
            return cmd;
        }

        /// <summary>
        ///     Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        ///     in the command text for each of the arguments and then executes that command.
        ///     Use this method instead of Query when you don't expect rows back. Such cases include
        ///     INSERTs, UPDATEs, and DELETEs.
        ///     You can set the Trace or TimeExecution properties of the connection
        ///     to profile execution.
        /// </summary>
        /// <param name="query">
        ///     The fully escaped SQL.
        /// </param>
        /// <param name="args">
        ///     Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        ///     The number of rows modified in the database as a result of this execution.
        /// </returns>

        public int Execute(string query, params object[] args)
        {
            var cmd = CreateCommand(query, args);

            if (TimeExecution)
            {
                if (_sw == null)
                {
                    _sw = Platform.StopwatchFactory.Create();
                }
                _sw.Reset();
                _sw.Start();
            }

            var r = cmd.ExecuteNonQuery();

            if (TimeExecution)
            {
                _sw.Stop();
                _elapsedMilliseconds += _sw.ElapsedMilliseconds;

                TraceListener.WriteLine("Finished in {0} ms ({1:0.0} s total)", _sw.ElapsedMilliseconds, _elapsedMilliseconds / 1000.0);
            }

            return r;
        }





        /// <summary>
        ///     Begins a new transaction. Call <see cref="Commit" /> to end the transaction.
        /// </summary>
        /// <example cref="System.InvalidOperationException">Throws if a transaction has already begun.</example>

        public void BeginTransaction()
        {
            // The BEGIN command only works if the transaction stack is empty, 
            //    or in other words if there are no pending transactions. 
            // If the transaction stack is not empty when the BEGIN command is invoked, 
            //    then the command fails with an error.
            // Rather than crash with an error, we will just ignore calls to BeginTransaction
            //    that would result in an error.
            if (Interlocked.CompareExchange(ref _transactionDepth, 1, 0) == 0)
            {
                try
                {
                    Execute("begin transaction");
                }
                catch (Exception ex)
                {
                    var sqlExp = ex as SQLiteException;
                    if (sqlExp != null)
                    {
                        // It is recommended that applications respond to the errors listed below 
                        //    by explicitly issuing a ROLLBACK command.
                        // TODO: This rollback failsafe should be localized to all throw sites.
                        switch (sqlExp.Result)
                        {
                            case Result.IOError:
                            case Result.Full:
                            case Result.Busy:
                            case Result.NoMem:
                            case Result.Interrupt:
                                RollbackTo(null, true);
                                break;
                        }
                    }
                    else
                    {
                        // Call decrement and not VolatileWrite in case we've already 
                        //    created a transaction point in SaveTransactionPoint since the catch.
                        Interlocked.Decrement(ref _transactionDepth);
                    }

                    throw;
                }
            }
            else
            {
                // Calling BeginTransaction on an already open transaction is invalid
                throw new InvalidOperationException("Cannot begin a transaction while already in a transaction.");
            }
        }

        /// <summary>
        ///     Creates a savepoint in the database at the current point in the transaction timeline.
        ///     Begins a new transaction if one is not in progress.
        ///     Call <see cref="RollbackTo" /> to undo transactions since the returned savepoint.
        ///     Call <see cref="Release" /> to commit transactions after the savepoint returned here.
        ///     Call <see cref="Commit" /> to end the transaction, committing all changes.
        /// </summary>
        /// <returns>A string naming the savepoint.</returns>

        public string SaveTransactionPoint()
        {
            var depth = Interlocked.Increment(ref _transactionDepth) - 1;
            var retVal = "S" + _rand.Next(short.MaxValue) + "D" + depth;

            try
            {
                Execute("savepoint " + retVal);
            }
            catch (Exception ex)
            {
                var sqlExp = ex as SQLiteException;
                if (sqlExp != null)
                {
                    // It is recommended that applications respond to the errors listed below 
                    //    by explicitly issuing a ROLLBACK command.
                    // TODO: This rollback failsafe should be localized to all throw sites.
                    switch (sqlExp.Result)
                    {
                        case Result.IOError:
                        case Result.Full:
                        case Result.Busy:
                        case Result.NoMem:
                        case Result.Interrupt:
                            RollbackTo(null, true);
                            break;
                    }
                }
                else
                {
                    Interlocked.Decrement(ref _transactionDepth);
                }

                throw;
            }

            return retVal;
        }

        /// <summary>
        ///     Rolls back the transaction that was begun by <see cref="BeginTransaction" /> or <see cref="SaveTransactionPoint" />
        ///     .
        /// </summary>

        public void Rollback()
        {
            RollbackTo(null, false);
        }

        /// <summary>
        ///     Rolls back the savepoint created by <see cref="BeginTransaction" /> or SaveTransactionPoint.
        /// </summary>
        /// <param name="savepoint">
        ///     The name of the savepoint to roll back to, as returned by <see cref="SaveTransactionPoint" />.
        ///     If savepoint is null or empty, this method is equivalent to a call to <see cref="Rollback" />
        /// </param>

        public void RollbackTo(string savepoint)
        {
            RollbackTo(savepoint, false);
        }

        /// <summary>
        ///     Rolls back the transaction that was begun by <see cref="BeginTransaction" />.
        /// </summary>
        /// <param name="savepoint">the savepoint name/key</param>
        /// <param name="noThrow">true to avoid throwing exceptions, false otherwise</param>
        private void RollbackTo(string savepoint, bool noThrow)
        {
            // Rolling back without a TO clause rolls backs all transactions 
            //    and leaves the transaction stack empty.   
            try
            {
                if (string.IsNullOrEmpty(savepoint))
                {
                    if (Interlocked.Exchange(ref _transactionDepth, 0) > 0)
                    {
                        Execute("rollback");
                    }
                }
                else
                {
                    DoSavePointExecute(savepoint, "rollback to ");
                }
            }
            catch (SQLiteException)
            {
                if (!noThrow)
                {
                    throw;
                }
            }
            // No need to rollback if there are no transactions open.
        }

        /// <summary>
        ///     Releases a savepoint returned from <see cref="SaveTransactionPoint" />.  Releasing a savepoint
        ///     makes changes since that savepoint permanent if the savepoint began the transaction,
        ///     or otherwise the changes are permanent pending a call to <see cref="Commit" />.
        ///     The RELEASE command is like a COMMIT for a SAVEPOINT.
        /// </summary>
        /// <param name="savepoint">
        ///     The name of the savepoint to release.  The string should be the result of a call to
        ///     <see cref="SaveTransactionPoint" />
        /// </param>

        public void Release(string savepoint)
        {
            DoSavePointExecute(savepoint, "release ");
        }

        private void DoSavePointExecute(string savePoint, string cmd)
        {
            // Validate the savepoint
            var firstLen = savePoint.IndexOf('D');
            if (firstLen >= 2 && savePoint.Length > firstLen + 1)
            {
                int depth;
                if (int.TryParse(savePoint.Substring(firstLen + 1), out depth))
                {
                    // TODO: Mild race here, but inescapable without locking almost everywhere.
                    if (0 <= depth && depth < _transactionDepth)
                    {
                        Platform.VolatileService.Write(ref _transactionDepth, depth);
                        Execute(cmd + savePoint);
                        return;
                    }
                }
            }

            throw new ArgumentException(
                "savePoint is not valid, and should be the result of a call to SaveTransactionPoint.", nameof(savePoint));
        }

        /// <summary>
        ///     Commits the transaction that was begun by <see cref="BeginTransaction" />.
        /// </summary>

        public void Commit()
        {
            if (Interlocked.Exchange(ref _transactionDepth, 0) != 0)
            {
                Execute("commit");
            }
            // Do nothing on a commit with no open transaction
        }

        /// <summary>
        ///     Executes
        ///     <paramref name="action" />
        ///     within a (possibly nested) transaction by wrapping it in a SAVEPOINT. If an
        ///     exception occurs the whole transaction is rolled back, not just the current savepoint. The exception
        ///     is rethrown.
        /// </summary>
        /// <param name="action">
        ///     The <see cref="Action" /> to perform within a transaction.
        ///     <paramref name="action" />
        ///     can contain any number
        ///     of operations on the connection but should never call <see cref="BeginTransaction" /> or
        ///     <see cref="Commit" />.
        /// </param>

        public void RunInTransaction(Action action)
        {
            try
            {
                var savePoint = SaveTransactionPoint();
                action();
                Release(savePoint);
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
        }

        #region Backup

        public string CreateDatabaseBackup(ISQLitePlatform platform)
        {
            ISQLiteApiExt sqliteApi = platform.SQLiteApi as ISQLiteApiExt;

            if (sqliteApi == null)
            {
                return null;
            }

            string destDBPath = this.DatabasePath + "." + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff");

            IDbHandle destDB;
            byte[] databasePathAsBytes = GetNullTerminatedUtf8(destDBPath);
            Result r = sqliteApi.Open(databasePathAsBytes, out destDB,
                (int)(SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite), IntPtr.Zero);

            if (r != Result.OK)
            {
                throw SQLiteException.New(r, String.Format("Could not open backup database file: {0} ({1})", destDBPath, r));
            }

            /* Open the backup object used to accomplish the transfer */
            IDbBackupHandle bHandle = sqliteApi.BackupInit(destDB, "main", this.Handle, "main");

            if (bHandle == null)
            {
                // Close the database connection 
                sqliteApi.Close(destDB);

                throw SQLiteException.New(r, String.Format("Could not initiate backup process: {0}", destDBPath));
            }

            /* Each iteration of this loop copies 5 database pages from database
            ** pDb to the backup database. If the return value of backup_step()
            ** indicates that there are still further pages to copy, sleep for
            ** 250 ms before repeating. */
            do
            {
                r = sqliteApi.BackupStep(bHandle, 5);

                if (r == Result.OK || r == Result.Busy || r == Result.Locked)
                {
                    sqliteApi.Sleep(250);
                }
            } while (r == Result.OK || r == Result.Busy || r == Result.Locked);

            /* Release resources allocated by backup_init(). */
            r = sqliteApi.BackupFinish(bHandle);

            if (r != Result.OK)
            {
                // Close the database connection 
                sqliteApi.Close(destDB);

                throw SQLiteException.New(r, String.Format("Could not finish backup process: {0} ({1})", destDBPath, r));
            }

            // Close the database connection 
            sqliteApi.Close(destDB);

            return destDBPath;
        }

        #endregion

        ~SQLiteConnection()
        {
            Dispose(false);
        }


        protected void Dispose(bool disposing)
        {
            Close();
        }


        public void Close()
        {
            if (_open && Handle != NullHandle)
            {
                try
                {
                    var r = Platform.SQLiteApi.Close(Handle);
                    if (r != Result.OK)
                    {
                        var msg = Platform.SQLiteApi.Errmsg16(Handle);
                        throw SQLiteException.New(r, msg);
                    }
                }
                finally
                {
                    Handle = NullHandle;
                    _open = false;
                }
            }
        }
    }
}