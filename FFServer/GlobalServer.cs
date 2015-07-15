using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerCore;

namespace FFServer
{
    class GlobalServer
    {
        Thread _theThread;
        GlobalClient _gc;
        
        public event EventHandler<AccountInfoResponseArgs> OnAccountInfoResponse;

        public GlobalServer()
        {
            _theThread = new Thread(new ThreadStart(GlobalServerManagerThread));
            _theThread.Name = "Global Server Manager";
            _theThread.Start();
        }

        void GlobalServerManagerThread()
        {
            while (true)
            {
                if (_gc == null || !_gc.Connected)
                {
                    _gc = new GlobalClient();
                    _gc.OnAccountInfoResponse += OnAccountInfoResponse;

                   
                    _gc.Connect("127.0.0.1", 1789);
                    if (_gc.Connected)
                        LogThread.Log("Connected to global server", LogThread.LogMessageType.Normal, true);
                }
                else
                {
                    _gc.Update();
                }


                Thread.Sleep(1000);
            }
        }


        public void RequestAccountInfo(int clientKey, string email, string password)
        {
            _gc.RequestAccountInfo(clientKey, email, password);
        }
    }
}
