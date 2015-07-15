using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerCore
{    
    public class Task
    {
        protected Connection _client;
        protected object _args;
    }

    public abstract class TaskProcessor
    {
        List<Task> _tasks;
        Mutex _tasksLock;
        bool _processing;

        public TaskProcessor()
        {
            _tasks = new List<Task>();
            _tasksLock = new Mutex();
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

        abstract public void ProcessTask(Task t);
    }
}
