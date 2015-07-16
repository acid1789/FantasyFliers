using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace ServerCore
{
    public class GameClient : Connection
    {
        public enum GCPacketType
        {
            AccountRequest = 1000,
            AccountResponse,
        }

        public event EventHandler<AccountRequestArgs> OnAccountRequest;
        public event EventHandler OnAccountResponse;

        #region Account Info
        int _accountId;
        string _displayName;
        int _hardCurrency;
        #endregion


        public GameClient()
            : base(null)
        {
        }

        public GameClient(Socket s)
            : base(s)
        {
            _sessionKey = (int)DateTime.Now.Ticks;
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();

            _packetHandlers[(ushort)GCPacketType.AccountRequest] = AccountRequestHandler;
            _packetHandlers[(ushort)GCPacketType.AccountResponse] = AccountResponseHandler;
        }

        void BeginPacket(GCPacketType type)
        {
            LogThread.Log(string.Format("BeginPacket({0})", type), LogThread.LogMessageType.Debug);
            BeginPacket((ushort)type);
        }

        #region Packet Construction        
        public void SendAccountRequest(string email, string pass)
        {
            BeginPacket(GCPacketType.AccountRequest);

            WriteUTF8String(email);
            WriteUTF8String(pass);

            SendPacket();
        }

        public void SendAccountResponse(int sessionKey)
        {
            BeginPacket(GCPacketType.AccountResponse);

            _outgoingBW.Write(sessionKey);

            SendPacket();
        }
        #endregion

        #region Packet Handlers
        void AccountRequestHandler(BinaryReader br)
        {
            string email = ReadUTF8String(br);
            string pass = ReadUTF8String(br);

            AccountRequestArgs args = new AccountRequestArgs();
            args.Email = email;
            args.Password = pass;
            OnAccountRequest(this, args);
        }

        void AccountResponseHandler(BinaryReader br)
        {
            _sessionKey = br.ReadInt32();
            OnAccountResponse(this, null);
        }
        #endregion

        #region Accessors
        public int AccountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }

        public int HardCurrency
        {
            get { return _hardCurrency; }
            set { _hardCurrency = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }
        #endregion
    }

    #region Args Classes
    public class AccountRequestArgs : EventArgs
    {
        public string Email;
        public string Password;
    }
    #endregion
}
