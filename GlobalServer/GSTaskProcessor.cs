using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using ServerCore;

namespace GlobalServer
{
    class GSTask : Task
    {
        public enum Type
        {
            AccountInfoRequest,
            AccountInfoProcess,
        }

        public Type TaskType;
        public DBQuery Query;

        public GlobalClient Client
        {
            get { return (GlobalClient)_client; }
            set { _client = value; }
        }

        public object Args
        {
            get { return _args; }
            set { _args = value; }
        }
    }

    class PendingQuery
    {
        public enum QueryType
        {
            AccountInfo
        }

        QueryType _type;
        GSTask _task;

        public PendingQuery(QueryType type, GSTask task)
        {
            _type = type;
            _task = task;
        }

        public QueryType Type
        {
            get { return _type; }
        }

        public GSTask Task
        {
            get { return _task; }
        }
    }

    class GSTaskProcessor : TaskProcessor
    {
        delegate void TaskHandler(GSTask task);

        Dictionary<GSTask.Type, TaskHandler> _taskHandlers;

        Dictionary<long, GSTask> _pendingQueries;
        Mutex _pqLock;

        public GSTaskProcessor()
            : base()
        {
            _taskHandlers = new Dictionary<GSTask.Type, TaskHandler>();
            _taskHandlers[GSTask.Type.AccountInfoRequest] = AccountInfoRequestHandler;
            _taskHandlers[GSTask.Type.AccountInfoProcess] = AccountInfoProcessHandler;

            _pendingQueries = new Dictionary<long, GSTask>();
            _pqLock = new Mutex();

            GlobalServer.Database.OnQueryComplete += new EventHandler(Database_OnQueryComplete);
        }

        public override void ProcessTask(Task t)
        {
            GSTask gst = (GSTask)t;
            if (gst != null)
            {
                // Call the task handler, this will throw an exception if the handler isnt registered.
                _taskHandlers[gst.TaskType](gst);
            }
            else
            {
                throw new InvalidDataException("ProcessTask was given an invalid Task object");
            }
        }

        void Database_OnQueryComplete(object sender, EventArgs e)
        {
            DBQuery q = (DBQuery)sender;

            _pqLock.WaitOne();
            GSTask task = _pendingQueries[q.Key];
            _pendingQueries.Remove(q.Key);
            _pqLock.ReleaseMutex();

            // reschedule the task to deal with the new data
            AddTask(task);
        }

        #region Task Handlers
        void AccountInfoRequestHandler(GSTask task)
        {
            // Fetch account from the database
            AccountInfoRequestArgs args = (AccountInfoRequestArgs)task.Args;
            long key = DateTime.Now.Ticks;
            string sql = string.Format("SELECT * FROM accounts WHERE email=\"{0}\";", args.Email);
            DBQuery q = new DBQuery(sql, true, key);
            GlobalServer.Database.AddQuery(q);

            task.TaskType = GSTask.Type.AccountInfoProcess;
            task.Query = q;

            _pqLock.WaitOne();
            _pendingQueries[key] = task;
            _pqLock.ReleaseMutex();
        }

        void AccountInfoProcessHandler(GSTask task)
        {
            int accountId = -1;
            string displayName = "";
            int hardCurrency = 0;
            
            AccountInfoRequestArgs args = (AccountInfoRequestArgs)task.Args;

            if (task.Query.Rows.Count > 0)
            {
                // Found the account, check the password
                object[] row = task.Query.Rows[0];
                accountId = (int)row[0];
                string pw = row[2].ToString();
                if (pw == args.Password)
                {
                    // password match
                    displayName = row[3].ToString();
                    hardCurrency = (int)row[4];
                }
                else
                {
                    // password mismatch - displayName stays empty but accountId is filled in
                }
            }
            else
            {
                // Account does not exist - accountId remains invalid                
            }
            task.Client.SendAccountInfo(args.ClientKey, accountId, displayName, hardCurrency); 
        }
        #endregion
    }
}
