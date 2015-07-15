﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

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
            BeginPacket((ushort)gt);
        }

        #region Packet Construction
        public void RequestAccountInfo(int clientKey, string email, string password)
        {
            BeginPacket(GPacketType.AccountInfoRequest);

            _outgoingBW.Write(clientKey);
            WriteUTF8String(email);
            WriteUTF8String(password);

            SendPacket();
        }

        public void SendAccountInfo(int clientKey, int accountId, string displayName, int hardCurrency)
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
            args.ClientKey = br.ReadInt32();
            args.Email = ReadUTF8String(br);
            args.Password = ReadUTF8String(br);
            OnAccountInfoRequest(this, args);
        }

        void AccountInfoResponseHandler(BinaryReader br)
        {
            AccountInfoResponseArgs args = new AccountInfoResponseArgs();
            args.ClientKey = br.ReadInt32();
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
        public int ClientKey;
        public string Email;
        public string Password;
    }

    public class AccountInfoResponseArgs : EventArgs
    {
        public int ClientKey;
        public int AccountId;
        public int HardCurrency;
        public string DisplayName;
    }
    #endregion
}