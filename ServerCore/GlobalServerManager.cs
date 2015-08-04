using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class GlobalServerManager
    {
        Thread _theThread;
        GlobalClient _gc;
        string _address;
        int _port;
        
        public event EventHandler<AccountInfoResponseArgs> OnAccountInfoResponse;

        public GlobalServerManager(string address, int port)
        {
            _address = address;
            _port = port;

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


                    _gc.Connect(_address, _port);
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


        public void RequestAccountInfo(uint clientKey, string email, string password, string displayName)
        {
            _gc.RequestAccountInfo(clientKey, email, password, displayName);
        }
    }
}
