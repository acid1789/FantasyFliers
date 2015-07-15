using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerCore
{
    public class ServerBase
    {
        DatabaseThread _db;
        ListenThread _lt;
        InputThread _inputThread;
        TaskProcessor _taskProcessor;

        public ServerBase(int listenPort, TaskProcessor tp, string dbConnectString)
        {
            _taskProcessor = tp;

            // Start log thread
            LogThread.Initialize();

            // Start database thread
            _db = new DatabaseThread(dbConnectString);

            // Start listen thread
            _lt = new ListenThread(listenPort);

            // Start input thread
            _inputThread = new InputThread();
        }

        public void Run()
        {
            // Allow connections
            _lt.Start();

            // Process
            _taskProcessor.Process();

            // Shutdown
        }

        #region Accessors
        public ListenThread ListenThread
        {
            get { return _lt; }
        }

        public InputThread InputThread
        {
            get { return _inputThread; }
        }

        public DatabaseThread Database
        {
            get { return _db; }
        }

        public TaskProcessor TaskProcessor
        {
            get { return _taskProcessor; }
        }
        #endregion

    }
}
