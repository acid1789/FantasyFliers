using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TestClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            tbEmail.Text = "ron@ungroundedgames.com";
            tbPassword.Text = "test";
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            // Connect to game server
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect("127.0.0.1", 1255);

            // Send Credentials
            const int PACKET_MARKER = 'U' | 'G' << 8 | 'G' << 16 | '$' << 24;
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write(PACKET_MARKER);
            bw.Write((ushort)1000);

            byte[] email = Encoding.UTF8.GetBytes(tbEmail.Text);
            byte[] pass = Encoding.UTF8.GetBytes(tbPassword.Text);

            bw.Write(email.Length);
            bw.Write(email);
            bw.Write(pass.Length);
            bw.Write(pass);

            byte[] packetData = ms.ToArray();
            s.Send(packetData);

            // Wait for response
            bool gotResponse = false;
            while (!gotResponse)
            {
                if (s.Available > 0)
                {
                    byte[] data = new byte[s.Available];
                    s.Receive(data);

                    gotResponse = ProcessData(data);
                }
                Thread.Sleep(100);
            }

            // Update status
        }

        bool ProcessData(byte[] data)
        {
            bool gotResponse = false;

            // Find the packet marker
            int packetStart = 0;
            while (packetStart < data.Length - 4)
            {
                if (data[packetStart] == 'U' && data[packetStart + 1] == 'G' && data[packetStart + 2] == 'G' && data[packetStart + 3] == '$')
                {
                    break;
                }
            }

            MemoryStream ms = new MemoryStream(data);
            ms.Seek(packetStart, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(ms);

            int marker = br.ReadInt32();
            ushort packetType = br.ReadUInt16();
            Debug.WriteLine("Got packet: " + packetType);

            switch (packetType)
            {
                case 1001:  // Credentials Response
                    gotResponse = true;
                    break;
                default:
                    break;
            }

            int bytesProcessed = (int)ms.Position;
            if (bytesProcessed < data.Length)
            {
                int remaining = data.Length - bytesProcessed;
                byte[] remainingData = new byte[remaining];
                Buffer.BlockCopy(data, bytesProcessed, remainingData, 0, remaining);
                if (ProcessData(remainingData))
                    gotResponse = true;
            }

            return gotResponse;
        }

    }
}
