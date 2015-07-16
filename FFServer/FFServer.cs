using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerCore;
using FFNetwork;

namespace FFServer
{
    class FFServer : GameServer
    {
        public FFServer(int listenPort, string dbConnectionString, string globalAddress, int globalPort) : base(listenPort, dbConnectionString, globalAddress, globalPort)
        {
            LogThread.AlwaysPrintToConsole = true;
            TaskProcessor = new FFTaskProcessor(this);
        }

        public override GameClient CreateClient(System.Net.Sockets.Socket s)
        {
            FFClient client = new FFClient(s);

            // Register FF specific handlers

            return client;
        }

        #region Client Event Handlers
        #endregion

        #region Accessors
        #endregion
    }
}
