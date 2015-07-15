using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerCore;

namespace FFServer
{
    class FFServer
    {
        static ServerBase _server;
        static GlobalServer _gs;

        static Random _random;

        static void Main(string[] args)
        {
            // Start the global server manager
            _gs = new GlobalServer();
            _gs.OnAccountInfoResponse += new EventHandler<AccountInfoResponseArgs>(_gs_OnAccountInfoResponse);

            // Start the server base
            _server = new ServerBase(1255, new FFTaskProcessor(), null);
            LogThread.AlwaysPrintToConsole = true;
            
            // Register listen handler
            _server.ListenThread.OnConnectionAccepted += new EventHandler<SocketArg>(lt_OnConnectionAccepted);

            // Run the server
            _server.Run();

            // Start logging thread
            LogThread.Initialize();
        }

        static void lt_OnConnectionAccepted(object sender, SocketArg e)
        {
            FFClient client = new FFClient(e.Socket);

            client.OnCredentialsRequest += new EventHandler<CredentialsRequestArgs>(client_OnCredentialsRequest);

            _server.InputThread.AddConnection(client);
        }

        #region Client Event Handlers
        static void client_OnCredentialsRequest(object sender, CredentialsRequestArgs e)
        {
            FFTask t = new FFTask();
            t.TaskType = FFTask.Type.CredentialsRequest;
            t.Client = (FFClient)sender;
            t.Args = e;

            _server.TaskProcessor.AddTask(t);
        }
        #endregion

        #region Global Server Event Handlers
        static void _gs_OnAccountInfoResponse(object sender, AccountInfoResponseArgs e)
        {
            FFTask t = new FFTask();
            t.TaskType = FFTask.Type.AccountInfoResponse;
            t.Client = null;
            t.Args = e;
        }
        #endregion

        #region Accessors
        public static Random Random
        {
            get { return _random; }
        }

        public static GlobalServer GlobalServer
        {
            get { return _gs; }
        }

        public static InputThread InputThread
        {
            get { return _server.InputThread; }
        }
        #endregion
    }
}
