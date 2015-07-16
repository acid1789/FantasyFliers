using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServerCore
{
    public class GSTask : Task
    {
        public enum GSTType
        {
            CredentialsRequest,
            AccountInfoResponse,
        }
        
        public GameClient Client
        {
            get { return (GameClient)_client; }
            set { _client = value; }
        }

        public object Args
        {
            get { return _args; }
            set { _args = value; }
        }
    }

    public class GSTaskProcessor : TaskProcessor
    {
        GameServer _server;

        public GSTaskProcessor(GameServer server)
            : base()
        {
            _server = server;

            _taskHandlers[(int)GSTask.GSTType.CredentialsRequest] = CredentialsRequestHandler;
            _taskHandlers[(int)GSTask.GSTType.AccountInfoResponse] = AccountInfoResponseHandler;
        }

        #region Task Handlers
        void CredentialsRequestHandler(Task t)
        {
            GSTask task = (GSTask)t;
            // Forward credentials request onto the global server
            AccountRequestArgs args = (AccountRequestArgs)task.Args;
            _server.GlobalServer.RequestAccountInfo(task.Client.SessionKey, args.Email, args.Password);
        }

        void AccountInfoResponseHandler(Task t)
        {
            GSTask task = (GSTask)t;
            AccountInfoResponseArgs args = (AccountInfoResponseArgs)task.Args;
            GameClient client = (GameClient)_server.InputThread.FindClient(args.ClientKey);
            if (args.AccountId < 0)
            {
                // Account doesnt exist
                client.SendAccountResponse(-1);
            }
            else if (args.DisplayName == null || args.DisplayName.Length <= 0)
            {
                // Account exists but password is wrong
                client.SendAccountResponse(-2);
            }
            else
            {
                // Valid account
                client.SendAccountResponse(args.ClientKey);

                // Store stuff
                client.AccountId = args.AccountId;
                client.HardCurrency = args.HardCurrency;
                client.DisplayName = args.DisplayName;
            }
        }
        #endregion

    }
}
