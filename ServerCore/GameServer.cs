using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class GameServer : ServerBase
    {
        GlobalServerManager _gs;

        public GameServer(int listenPort, string dbConnectionString, string globalAddress, int globalPort)
            : base(listenPort, dbConnectionString)
        {
            // Start the global server manager
            _gs = new GlobalServerManager(globalAddress, globalPort);
            _gs.OnAccountInfoResponse += new EventHandler<AccountInfoResponseArgs>(_gs_OnAccountInfoResponse);            

            ListenThread.OnConnectionAccepted += new EventHandler<SocketArg>(lt_OnConnectionAccepted);            
        }

        public abstract GameClient CreateClient(Socket s);
    
        void lt_OnConnectionAccepted(object sender, SocketArg e)
        {
            GameClient client = CreateClient(e.Socket);

            client.OnAccountRequest += new EventHandler<AccountRequestArgs>(client_OnCredentialsRequest);
            client.OnChatMessage += new EventHandler<ChatMessageArgs>(client_OnChatMessage);

            InputThread.AddConnection(client);
        }

        #region Server Tasks
        public void FetchChatInfo(GameClient client)
        {
            GSTask t = new GSTask();
            t.Type = (int)GSTask.GSTType.ChatBlockList_Fetch;
            t.Client = client;
            TaskProcessor.AddTask(t);
        }
        #endregion

        #region Client Event Handlers
        void client_OnCredentialsRequest(object sender, AccountRequestArgs e)
        {
            GSTask t = new GSTask();
            t.Type = (int)GSTask.GSTType.CredentialsRequest;
            t.Client = (GameClient)sender;
            t.Args = e;

            TaskProcessor.AddTask(t);
        }

        void client_OnChatMessage(object sender, ChatMessageArgs e)
        {
            GSTask t = new GSTask();
            t.Type = (int)GSTask.GSTType.ChatMessage;
            t.Client = (GameClient)sender;
            t.Args = e;

            TaskProcessor.AddTask(t);
        }
        #endregion

        #region Global Server Event Handlers
        void _gs_OnAccountInfoResponse(object sender, AccountInfoResponseArgs e)
        {
            GSTask t = new GSTask();
            t.Type = (int)GSTask.GSTType.AccountInfoResponse;
            t.Client = null;
            t.Args = e;

            TaskProcessor.AddTask(t);
        }
        #endregion

        #region Accessors
        public GlobalServerManager GlobalServer
        {
            get { return _gs; }
        }
        #endregion
    }
}
