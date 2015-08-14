using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerCore;

namespace FFServer
{
    class Program
    {
        static FFServer _server;

        static void Main(string[] args)
        {
            LogThread.AlwaysPrintToConsole = true;
            _server = new FFServer(1255, "server=127.0.0.1;uid=root;pwd=ugg$;database=ff_server;", "127.0.0.1", 1789);

            _server.Run();

            LogThread.GetLog().Shutdown();
        }

        #region Accessors
        #endregion
    }
}
