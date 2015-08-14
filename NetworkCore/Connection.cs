using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace NetworkCore
{
    public class Connection
    {
        public const int PACKET_MARKER = 'U' | 'G' << 8 | 'G' << 16 | '$' << 24;
        public enum ConnStatus
        {
            New,
            Authorized,
            Disconnected,
            Closed
        }

        public enum PacketType
        {
            Ping,
            Pong
        }

        protected Socket _socket;
        protected ConnStatus _status;

        protected List<byte> _pendingData;
        protected DateTime _lastSeen;
        protected DateTime _lastSent;

        protected delegate void PacketHandler(BinaryReader br);
        protected Dictionary<UInt16, PacketHandler> _packetHandlers;

        protected MemoryStream _outgoingPacket;
        protected BinaryWriter _outgoingBW;

        protected uint _sessionKey;
        
        public Connection(Socket s)
        {
            _socket = s;
            _status = ConnStatus.New;

            _lastSeen = DateTime.Now;
            _lastSent = DateTime.Now;

            _packetHandlers = new Dictionary<UInt16, PacketHandler>();
            RegisterPacketHandlers();
        }

        public void Close()
        {
            if( _socket != null )
                _socket.Close();
            _socket = null;
            _status = ConnStatus.Closed;
        }

        protected virtual void RegisterPacketHandlers()
        {
            _packetHandlers[(ushort)PacketType.Ping] = PingHandler;
            _packetHandlers[(ushort)PacketType.Pong] = PongHandler;
        }

        public void Update()
        {
            if (_socket != null)
            {
                if (!_socket.Connected && _status != ConnStatus.Disconnected)
                {
                    _socket = null;
                    _status = ConnStatus.Disconnected;
                }

                if (_socket.Connected)
                {
                    // Read in packet data
                    if (_socket.Available > 0)
                    {
                        _lastSeen = DateTime.Now;

                        byte[] data = new byte[_socket.Available];
                        int bytesReceived = _socket.Receive(data);

                        if (_pendingData != null)
                        {
                            _pendingData.AddRange(data);
                            data = _pendingData.ToArray();
                            _pendingData = null;
                        }

                        ProcessPacketData(data);
                    }

                    // Are you still there?
                    double timeSinceSeen = (DateTime.Now - _lastSeen).TotalSeconds;
                    if (timeSinceSeen > 30 && (DateTime.Now - _lastSent).TotalSeconds > 15)
                    {
                        Ping();
                    }
                    if (timeSinceSeen > 60)
                    {
                        // Client is gone
                        _socket.Close();
                        _socket = null;
                        _status = ConnStatus.Disconnected;
                    }
                }
            }

            if (_pendingData != null)
            {
                byte[] packet = _pendingData.ToArray();
                _pendingData = null;
                ProcessPacketData(packet);
            }
        }

        void ProcessPacketData(byte[] data)
        {
            // Find the packet marker
            int packetStart = 0;
            while (packetStart < data.Length - 4)
            {
                if (data[packetStart] == 'U' && data[packetStart + 1] == 'G' && data[packetStart + 2] == 'G' && data[packetStart + 3] == '$')
                {
                    break;
                }
            }
            if (packetStart > 0)
                LogInterface.Log(string.Format("Connection threw away {0} bytes before packet marker", packetStart), LogInterface.LogMessageType.Debug);

            MemoryStream ms = new MemoryStream(data);
            ms.Seek(packetStart, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(ms);

            int marker = br.ReadInt32();
            ushort packetType = br.ReadUInt16();
            LogInterface.Log(string.Format("Processing packet type {0}", packetType), LogInterface.LogMessageType.Debug);

            if (_packetHandlers.ContainsKey(packetType))
            {
                _packetHandlers[packetType](br);

                int bytesProcessed = (int)ms.Position;
                if (bytesProcessed < data.Length)
                {
                    int remaining = data.Length - bytesProcessed;
                    byte[] remainingData = new byte[remaining];
                    Buffer.BlockCopy(data, bytesProcessed, remainingData, 0, remaining);
                    _pendingData = new List<byte>();
                    _pendingData.AddRange(remainingData);
                }
            }
            else
            {
                LogInterface.Log(string.Format("Unhandled packet type {0}", packetType), LogInterface.LogMessageType.Error, true);
            }
        }

        protected void BeginPacket(PacketType type)
        {
            LogInterface.Log(string.Format("BeginPacket({0})", type), LogInterface.LogMessageType.Debug);
            BeginPacket((ushort)type);
        }

        protected void BeginPacket(ushort type)
        {
            if (_outgoingPacket != null)
                throw new InvalidOperationException("Can not start a packet with one already in progress!");

            _outgoingPacket = new MemoryStream();
            _outgoingBW = new BinaryWriter(_outgoingPacket);

            _outgoingBW.Write(PACKET_MARKER);
            _outgoingBW.Write(type);
        }

        protected void SendPacket()
        {
            if (_outgoingPacket == null)
                throw new InvalidOperationException("Can not send packet that has not been started!");

            byte[] data = _outgoingPacket.ToArray();
            _socket.Send(data);

            _lastSent = DateTime.Now;
            LogInterface.Log(string.Format("SendPacket {0} bytes", data.Length), LogInterface.LogMessageType.Debug);

            _outgoingBW.Close();
            _outgoingBW = null;
            _outgoingPacket = null;
        }

        public void Connect(string address, int port)
        {
            if (_socket != null)
                throw new InvalidOperationException("Cant connect when a socket already exists");

            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(address, port);
            }
            catch (Exception ex)
            {
                LogInterface.Log(string.Format("Failed to connect to {0}:{1}\n{2}", address, port, ex.ToString()), LogInterface.LogMessageType.Error, true);
                _socket = null;
            }
        }

        public string ReadUTF8String(BinaryReader br)
        {
            string str = null;
            int len = br.ReadInt32();
            if (len > 0)
            {
                byte[] encoded = br.ReadBytes(len);
                str = Encoding.UTF8.GetString(encoded);
            }
            return str;
        }

        public void WriteUTF8String(string s, BinaryWriter bw = null)
        {
            if (bw == null)
                bw = _outgoingBW;

            if (s != null)
            {
                byte[] encoded = Encoding.UTF8.GetBytes(s);
                bw.Write(encoded.Length);
                bw.Write(encoded);
            }
            else
            {
                int length = 0;
                bw.Write(length);
            }
        }

        #region Packet Construction
        void Ping()
        {
            BeginPacket(PacketType.Ping);
            SendPacket();
        }
        #endregion

        #region Packet Handlers
        void PingHandler(BinaryReader br)
        {
            // Got a ping request from the other side, send pong
            BeginPacket(PacketType.Pong);
            SendPacket();
        }

        void PongHandler(BinaryReader br)
        {
        }
        #endregion

        #region Accesssors
        public ConnStatus Status
        {
            get { return _status; }
        }

        public bool Connected
        {
            get { return (_socket != null && _socket.Connected); }
        }

        public uint SessionKey
        {
            get { return _sessionKey; }
        }
        #endregion
    }
}
