using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetworkCore;

namespace ServerCore
{    
    public class Task
    {
        public int Type;
        public DBQuery Query;

        protected Connection _client;
        protected object _args;

        public Task()
        {
        }

        public Task(int type)
        {
            Type = type;
        }
        
        public object Args
        {
            get { return _args; }
            set { _args = value; }
        }
    }

    public abstract class TaskProcessor
    {
        List<Task> _tasks;
        Mutex _tasksLock;
        bool _processing;

        long _lastTicks;
        long _ticksModified;

        DatabaseThread _db;

        protected delegate void TaskHandler(Task task);
        protected Dictionary<int, TaskHandler> _taskHandlers;

        protected Dictionary<long, Task> _pendingQueries;
        protected Mutex _pqLock;

        public TaskProcessor()
        {
            _tasks = new List<Task>();
            _tasksLock = new Mutex();

            _taskHandlers = new Dictionary<int, TaskHandler>();
            
            _pendingQueries = new Dictionary<long, Task>();
            _pqLock = new Mutex();
        }

        public void Database_OnQueryComplete(object sender, EventArgs e)
        {
            DBQuery q = (DBQuery)sender;

            _pqLock.WaitOne();
            LogInterface.Log("Finishing Query with key: " + q.Key, LogInterface.LogMessageType.Debug, true);
            Task task = _pendingQueries[q.Key];
            _pendingQueries.Remove(q.Key);
            _pqLock.ReleaseMutex();

            // reschedule the task to deal with the new data\
            if( task.Type >= 0 )
                AddTask(task);
        }

        long UniqueKey()
        {
            long ticks = DateTime.Now.Ticks;
            long key = ticks;
            if (_lastTicks == ticks)
                key += ++_ticksModified;
            else
                _ticksModified = 0;
            _lastTicks = ticks;
            return key;
        }

        public DBQuery AddDBQuery(string sql, Task task, bool read = true)
        {
            if( task == null )
                task = new Task(-1);

            long key = UniqueKey();
            DBQuery q = new DBQuery(sql, read, key);
            
            task.Query = q;

            _pqLock.WaitOne();
            LogInterface.Log("Adding Query with key: " + key, LogInterface.LogMessageType.Debug, true);
            _pendingQueries[key] = task;
            _pqLock.ReleaseMutex();

            _db.AddQuery(q);
            return q;
        }

        public void AddTask(Task t)
        {
            _tasksLock.WaitOne();
            _tasks.Add(t);
            _tasksLock.ReleaseMutex();
        }

        public void Process()
        {
            _processing = true;
            while (_processing)
            {
                try
                {
                    if (_tasks.Count > 0)
                    {
                        // Grab the first task
                        _tasksLock.WaitOne();
                        Task t = _tasks[0];
                        _tasks.RemoveAt(0);
                        _tasksLock.ReleaseMutex();

                        // Execute the task
                        ProcessTask(t);
                    }
                }
                catch (Exception ex)
                {
                    LogThread.Log(ex.ToString(), LogThread.LogMessageType.Error, false);
                }

                Thread.Sleep(10);
            }
        }

        void ProcessTask(Task t)
        {
            // Call the task handler, this will throw an exception if the handler isnt registered.
            LogThread.Log(string.Format("ProcessTask({0}) -> {1}", t.Type, _taskHandlers[t.Type].Method.Name), LogThread.LogMessageType.Debug);
            _taskHandlers[t.Type](t);
        }

        #region Accessors
        public DatabaseThread Database
        {
            get { return _db; }
            set { _db = value; }       
        }
        #endregion
    }
}
