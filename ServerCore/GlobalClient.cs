using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using NetworkCore;

namespace ServerCore
{
    public class GlobalClient : Connection
    {
        public enum GPacketType
        {
            AccountInfoRequest = 50000,
            AccountInfoResponse,

        }

        public event EventHandler<AccountInfoRequestArgs> OnAccountInfoRequest;
        public event EventHandler<AccountInfoResponseArgs> OnAccountInfoResponse;


        public GlobalClient() : base (null)
        {
        }

        public GlobalClient(Socket s)
            : base(s)
        {
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();

            _packetHandlers[(ushort)GPacketType.AccountInfoRequest] = AccountInfoRequestHandler;
            _packetHandlers[(ushort)GPacketType.AccountInfoResponse] = AccountInfoResponseHandler;
        }

        void BeginPacket(GPacketType gt)
        {
            LogThread.Log(string.Format("BeginPacket({0})", gt), LogThread.LogMessageType.Debug);
            BeginPacket((ushort)gt);
        }

        #region Packet Construction
        public void RequestAccountInfo(uint clientKey, string email, string password, string displayName)
        {
            BeginPacket(GPacketType.AccountInfoRequest);

            _outgoingBW.Write(clientKey);
            WriteUTF8String(email);
            WriteUTF8String(password);
            WriteUTF8String(displayName);

            SendPacket();
        }

        public void SendAccountInfo(uint clientKey, int accountId, string displayName, int hardCurrency)
        {
            BeginPacket(GPacketType.AccountInfoResponse);

            _outgoingBW.Write(clientKey);
            _outgoingBW.Write(accountId);
            _outgoingBW.Write(hardCurrency);
            WriteUTF8String(displayName);

            SendPacket();
        }
        #endregion

        #region Packet Handlers
        void AccountInfoRequestHandler(BinaryReader br)
        {
            AccountInfoRequestArgs args = new AccountInfoRequestArgs();
            args.ClientKey = br.ReadUInt32();
            args.Email = ReadUTF8String(br);
            args.Password = ReadUTF8String(br);
            args.DisplayName = ReadUTF8String(br);
            OnAccountInfoRequest(this, args);
        }

        void AccountInfoResponseHandler(BinaryReader br)
        {
            AccountInfoResponseArgs args = new AccountInfoResponseArgs();
            args.ClientKey = br.ReadUInt32();
            args.AccountId = br.ReadInt32();
            args.HardCurrency = br.ReadInt32();
            args.DisplayName = ReadUTF8String(br);
            OnAccountInfoResponse(this, args);
        }
        #endregion
    }

    #region Args Classes
    public class AccountInfoRequestArgs : EventArgs
    {
        public uint ClientKey;
        public string Email;
        public string Password;
        public string DisplayName;
    }

    public class AccountInfoResponseArgs : EventArgs
    {
        public uint ClientKey;
        public int AccountId;
        public int HardCurrency;
        public string DisplayName;
    }
    #endregion
}
