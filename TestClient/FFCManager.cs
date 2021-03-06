﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using FFNetwork;

namespace TestClient
{
    class FFCManager
    {
        FFClient _ffc;
        Thread _theThread;

        string _address;
        int _port;

        public event EventHandler SignInComplete;
        public event EventHandler OnChatChannels;
        public event EventHandler<NetworkCore.ChatMessageArgs> OnChatMessage;

        public FFCManager(string address, int port)
        {
            _address = address;
            _port = port;

            _theThread = new Thread(new ThreadStart(FFCManagerThreadFunc));
            _theThread.Name = "FFC Manager";
            _theThread.Start();
        }

        public void Destroy()
        {
            _theThread.Abort();
        }

        void FFCManagerThreadFunc()
        {
            while (true)
            {
                if (_ffc == null)
                {
                    _ffc = new FFClient();
                    _ffc.OnAccountResponse += new EventHandler(_ffc_OnAccountResponse);
                    _ffc.OnChatChannels += OnChatChannels;
                    _ffc.OnChatMessage += OnChatMessage;

                    _ffc.Connect(_address, _port);
                }

                if (_ffc != null && _ffc.Connected)
                    _ffc.Update();

                Thread.Sleep(100);
            }
        }

        void _ffc_OnAccountResponse(object sender, EventArgs e)
        {
            SignInComplete(_ffc, null);
        }

        public void SignIn(string email, string pass)
        {
            _ffc.SendAccountRequest(email, pass);
        }

        public void CreateAccount(string email, string password, string displayname)
        {
            _ffc.SendAccountRequest(email, password, displayname);
        }

        public void SendChat(int channel, string message)
        {
            _ffc.SendChat(channel, message, null);
        }
    }
}
