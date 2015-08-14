using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;

namespace ServerCore
{
    public class DBQuery
    {
        string _sqlString;
        bool _read;
        List<object[]> _rows;
        long _key;

        public DBQuery(string sql, bool read, long key)
        {
            _sqlString = sql;
            _read = read;
            _key = key;
        }

        #region Accessors
        public string SqlString
        {
            get { return _sqlString; }
        }

        public bool Read
        {
            get { return _read; }
        }

        public List<object[]> Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        public long Key
        {
            get { return _key; }
        }
        #endregion
    }

    public class DatabaseThread
    {
        Thread _theThread;

        List<DBQuery> _queries;
        Mutex _queriesLock;

        MySqlConnection _sql;
        string _dbConnectString;

        public event EventHandler OnQueryComplete;

        public DatabaseThread(string connectString)
        {
            _dbConnectString = connectString;

            _queries = new List<DBQuery>();
            _queriesLock = new Mutex();

            _theThread = new Thread(new ThreadStart(DatabaseThreadFunc));
            _theThread.Name = "Database Thread";
            _theThread.Start();
        }

        public void Destroy()
        {
            _theThread.Abort();
            _theThread = null;
        }

        public void AddQuery(DBQuery query)
        {
            _queriesLock.WaitOne();
            _queries.Add(query);
            _queriesLock.ReleaseMutex();
        }

        void DatabaseThreadFunc()
        {
            while (true)
            {
                ValidateConnection();

                if (_sql != null && _sql.State == ConnectionState.Open && _queries.Count > 0)
                {
                    _queriesLock.WaitOne();
                    DBQuery query = _queries[0];
                    _queries.RemoveAt(0);
                    _queriesLock.ReleaseMutex();

                    try
                    {
                        ExecuteQuery(query);
                    }
                    catch(Exception ex)
                    {
                        LogThread.Log(ex.ToString(), LogThread.LogMessageType.Error, true);

                        // When the query fails, its likely due to database disconnection. Reschedule this query to be done again.
                        AddQuery(query);
                    }
                }

                Thread.Sleep(100);
            }
        }

        void ValidateConnection()
        {
            if (_sql == null)
            {
                try
                {
                    _sql = new MySqlConnection();
                    _sql.ConnectionString = _dbConnectString;
                    _sql.Open();

                    Thread.Sleep(10);

                    while (_sql.State == ConnectionState.Connecting)
                        Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    LogThread.Log(ex.ToString(), LogThread.LogMessageType.Error, true);
                    _sql.Close();
                    _sql = null;
                }
            }
            else
            {
                if (_sql.State == ConnectionState.Closed || _sql.State == ConnectionState.Broken)
                {
                    _sql.Close();
                    _sql = null;
                }
            }
        }

        void ExecuteQuery(DBQuery q)
        {
            LogThread.Log(string.Format("ExecuteQuery - {0}", q.SqlString), LogThread.LogMessageType.Debug);
            MySqlCommand cmd = new MySqlCommand(q.SqlString, _sql);
            if (q.Read)
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                List<object[]> rows = new List<object[]>();
                while (reader.Read())
                {
                    object[] row = new object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader[i];
                    }
                    rows.Add(row);
                }
                reader.Close();
                q.Rows = rows;
            }
            else
            {
                cmd.ExecuteNonQuery();
            }

            OnQueryComplete(q, null);
        }
    }
}
