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
            _server = new FFServer(1255, null, "127.0.0.1", 1789);

            _server.Run();

            LogThread.Shutdown();
        }

        #region Accessors
        #endregion
    }
}
