using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class SocketArg : EventArgs
    {
        public Socket Socket;

        public SocketArg(Socket s)
        {
            Socket = s;
        }
    }

    public class ListenThread
    {
        Thread _theThread;
        int _port;

        public event EventHandler<SocketArg> OnConnectionAccepted;

        public ListenThread(int port)
        {
            _port = port;
        }

        public void Destroy()
        {
            if (_theThread != null)
            {
                _theThread.Abort();
                _theThread = null;
            }
        }

        public void Start()
        {
            if (_theThread == null)
            {
                _theThread = new Thread(new ThreadStart(ListenThreadFunc));
                _theThread.Name = "Listen Thread";
                _theThread.Start();
            }
        }
        
        void ListenThreadFunc()
        {
            Socket listenSocket = null;
            try
            {
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, _port);
                listenSocket.Bind(ep);
                LogThread.Log("ListenThread: Listening on port " + _port, LogThread.LogMessageType.Normal, true);

                while (true)
                {
                    listenSocket.Listen(10);
                    Socket conn = listenSocket.Accept();
                    if (conn != null && OnConnectionAccepted != null)
                    {
                        LogThread.Log("ListenThread: Connection Accepted", LogThread.LogMessageType.Debug);
                        OnConnectionAccepted(this, new SocketArg(conn));
                    }
                }
            }
            catch (Exception ex)
            {
                LogThread.Log(ex.ToString(), LogThread.LogMessageType.Error, true);

                listenSocket.Close();
                _theThread = null;
            }
        }
    }
}
