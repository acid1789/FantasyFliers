using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using ServerCore;

namespace FFServer
{
    public class FFClient : Connection
    {
        public enum FFPacketType
        {
            CredentialsRequest = 1000,
            CredentialsResponse
        }

        public event EventHandler<CredentialsRequestArgs> OnCredentialsRequest;


        public FFClient()
            : base(null)
        {
        }

        public FFClient(Socket s)
            : base(s)
        {
            _sessionKey = (int)DateTime.Now.Ticks;
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();

            _packetHandlers[(ushort)FFPacketType.CredentialsRequest] = CredentialsRequestHandler;
        }


        #region Packet Handlers
        void CredentialsRequestHandler(BinaryReader br)
        {
            string email = ReadUTF8String(br);
            string pass = ReadUTF8String(br);

            CredentialsRequestArgs args = new CredentialsRequestArgs();
            args.Email = email;
            args.Password = pass;
            OnCredentialsRequest(this, args);
        }
        #endregion
    }

    #region Args Classes
    public class CredentialsRequestArgs : EventArgs
    {
        public string Email;
        public string Password;
    }
    #endregion
}
