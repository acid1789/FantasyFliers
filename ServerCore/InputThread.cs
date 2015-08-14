using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using NetworkCore;

namespace ServerCore
{
    public class InputThread
    {
        Thread _theThread;

        List<Connection> _connections;
        Mutex _connectionsLock;

        public InputThread()
        {
            _connections = new List<Connection>();
            _connectionsLock = new Mutex();

            _theThread = new Thread(new ThreadStart(InputThreadFunc));
            _theThread.Name = "Input Thread";
            _theThread.Start();
        }

        public void AddConnection(Connection c)
        {
            _connectionsLock.WaitOne();
            _connections.Add(c);
            _connectionsLock.ReleaseMutex();
        }

        public Connection FindClient(uint clientKey)
        {
            _connectionsLock.WaitOne();
            Connection[] connections = _connections.ToArray();
            _connectionsLock.ReleaseMutex();

            foreach (Connection c in connections)
            {
                if (c.SessionKey == clientKey)
                    return c;
            }
            return null;
        }

        void InputThreadFunc()
        {
            while (true)
            {
                try
                {
                    _connectionsLock.WaitOne();
                    Connection[] connections = _connections.ToArray();
                    _connectionsLock.ReleaseMutex();

                    List<Connection> removeList = new List<Connection>();
                    foreach (Connection c in connections)
                    {
                        c.Update();
                        if (c.Status == Connection.ConnStatus.Closed)
                            removeList.Add(c);
                    }

                    _connectionsLock.WaitOne();
                    foreach( Connection c in removeList )
                    {
                        _connections.Remove(c);
                    }
                    _connectionsLock.ReleaseMutex();
                }
                catch (Exception ex)
                {
                    LogThread.Log(ex.ToString(), LogThread.LogMessageType.Error, true);
                }

                Thread.Sleep(10);
            }
        }

        #region Accesssors
        public Connection[] Clients
        {
            get
            {
                _connectionsLock.WaitOne();
                Connection[] connections = _connections.ToArray();
                _connectionsLock.ReleaseMutex();
                return connections;
            }
        }
        #endregion
    }
}
