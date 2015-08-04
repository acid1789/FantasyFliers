using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerCore;

namespace GlobalServer
{
    class GlobalServer
    {
        static ServerBase _server;

        static void Main(string[] args)
        {
            _server = new ServerBase(1789, "server=127.0.0.1;uid=root;pwd=ugg$;database=global;");
            _server.TaskProcessor = new GlobalTaskProcessor();
            LogThread.AlwaysPrintToConsole = true;

            _server.ListenThread.OnConnectionAccepted += new EventHandler<SocketArg>(lt_OnConnectionAccepted);

            _server.Run();
        }

        static void lt_OnConnectionAccepted(object sender, SocketArg e)
        {
            GlobalClient gclient = new GlobalClient(e.Socket);

            gclient.OnAccountInfoRequest += new EventHandler<AccountInfoRequestArgs>(gclient_OnAccountInfoRequest);

            _server.InputThread.AddConnection(gclient);
        }

        static void gclient_OnAccountInfoRequest(object sender, AccountInfoRequestArgs e)
        {
            GlobalTask gst = new GlobalTask();
            gst.Type = (int)GlobalTask.GlobalType.AccountInfoRequest;
            gst.Client = (GlobalClient)sender;
            gst.Args = e;

            _server.TaskProcessor.AddTask(gst);
        }

        #region Accessors
        public static DatabaseThread Database
        {
            get { return _server.Database; }
        }
        #endregion
    }
}
