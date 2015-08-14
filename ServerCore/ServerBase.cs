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

        public ServerBase(int listenPort, string dbConnectString)
        {
            // Start log thread
            LogThread log = new LogThread();

            // Start database thread
            if( dbConnectString != null )
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

            // Setup db callback
            if (_db != null)
            {
                _taskProcessor.Database = _db;
                _db.OnQueryComplete += new EventHandler(_taskProcessor.Database_OnQueryComplete);
            }

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
            set { _taskProcessor = value; }
        }
        #endregion

    }
}
