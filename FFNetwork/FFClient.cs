using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using ServerCore;

namespace FFNetwork
{
    public class FFClient : GameClient
    {
        public enum FFPacketType
        {
        }
        
        public FFClient()
            : base(null)
        {
        }

        public FFClient(Socket s)
            : base(s)
        {
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();
        }

        void BeginPacket(FFPacketType type)
        {
            LogThread.Log(string.Format("BeginPacket({0})", type), LogThread.LogMessageType.Debug);
            BeginPacket((ushort)type);
        }

        #region Packet Construction
        #endregion

        #region Packet Handlers
        #endregion

        #region Accessors
        #endregion
    }

    #region Args Classes
    #endregion
}
