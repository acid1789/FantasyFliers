using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerCore
{    
    public class Task
    {
        public int Type;
        public DBQuery Query;

        protected Connection _client;
        protected object _args;

    }

    public abstract class TaskProcessor
    {
        List<Task> _tasks;
        Mutex _tasksLock;
        bool _processing;

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
            Task task = _pendingQueries[q.Key];
            _pendingQueries.Remove(q.Key);
            _pqLock.ReleaseMutex();

            // reschedule the task to deal with the new data\
            if( task.Type >= 0 )
                AddTask(task);
        }

        public DBQuery AddDBQuery(string sql, Task task)
        {
            long key = DateTime.Now.Ticks;
            DBQuery q = new DBQuery(sql, true, key);
            _db.AddQuery(q);

            task.Query = q;

            _pqLock.WaitOne();
            _pendingQueries[key] = task;
            _pqLock.ReleaseMutex();
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
