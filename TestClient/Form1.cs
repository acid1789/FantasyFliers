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
using FFNetwork;

namespace TestClient
{
    public partial class Form1 : Form
    {
        enum ChatChannel
        {
            Global
        }

        class ChatEntry
        {
            public ChatChannel Channel;
            public string Message;

            public ChatEntry(ChatChannel channel, string msg)
            {
                Channel = channel;
                Message = msg;
            }
        }

        FFCManager _ffcm;
        List<ChatEntry> _chatEntries;
        ChatChannel _inputChannel;

        public Form1()
        {
            InitializeComponent();
            _inputChannel = ChatChannel.Global;

            _chatEntries = new List<ChatEntry>();

            _ffcm = new FFCManager("127.0.0.1", 1255);
            _ffcm.OnChatChannels += _ffcm_OnChatChannels;
            _ffcm.OnChatMessage += _ffcm_OnChatMessage;

            tbEmail.Text = "ron@ungroundedgames.com";
            tbPassword.Text = "test";
        }

        private void _ffcm_OnChatMessage(object sender, NetworkCore.ChatMessageArgs e)
        {
            IncommingChatMessage((ChatChannel)e.Channel, e.Message, e.Sender);
        }

        void EnableSignIn(bool enable)
        {
            if( InvokeRequired )
            {
                Invoke(new MethodInvoker(delegate() { EnableSignIn(enable); }));
            }
            else
            {                
                btnSignIn.Enabled = enable;
                tbEmail.Enabled = enable;
                tbPassword.Enabled = enable;
            }
        }

        void EnableCreateAccount(bool enable)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate() { EnableCreateAccount(enable); }));
            }
            else
            {
                btnCreateAccount.Visible = enable;
            }
        }

        void SetStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate() { SetStatus(status); }));
            }
            else
            {
                lblStatus.Text = status;
            }
        }

        void ShowWaitCursor(bool show)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate() { ShowWaitCursor(show); }));
            }
            else
            {
                UseWaitCursor = show;
            }
        }

        void EnableChat(bool enable)
        {
            if( InvokeRequired )
            {
                Invoke(new MethodInvoker(delegate () { EnableChat(enable); }));
            }
            else
            {
                btnSendChat.Enabled = enable;
                tbChatInput.Enabled = enable;
            }
        }

        void SetupChatChannels(uint channels)
        {
            if( InvokeRequired )
            {
                Invoke(new MethodInvoker(delegate() { SetupChatChannels(channels); }));
            }
            else
            {
                tabChatTabs.TabPages.Clear();
                tabChatTabs.TabPages.Add("All");

                if( (channels & 0x00000001) != 0 )
                {
                    tabChatTabs.TabPages.Add("Global");
                    IncommingChatMessage(ChatChannel.Global, "Joined Global Channel", null);
                }
            }
        }

        void IncommingChatMessage(ChatChannel channel, string message, string sender)
        {
            if( InvokeRequired )
            {
                Invoke(new MethodInvoker(delegate() { IncommingChatMessage(channel, message, sender); }));
            }
            else
            {
                string formattedMessage = message;
                if( sender != null )
                {
                    formattedMessage = string.Format("[{0}] {1}: {2}", channel.ToString(), sender, message);
                }

                ChatEntry ce = new ChatEntry(channel, formattedMessage);
                _chatEntries.Add(ce);
                FilterChat();
            }
        }
        
        ChatChannel GetChatChannel()
        {
            return _inputChannel;
        }

        void FilterChat()
        {
            List<string> chatLines = new List<string>();

            int currentTab = tabChatTabs.SelectedIndex;
            if( currentTab == 0 )
            {
                // All tab, put everything
                foreach (ChatEntry entry in _chatEntries)
                {
                    chatLines.Add(entry.Message);
                }
            }
            else
            {
                ChatChannel channel = GetChatChannel();
                foreach( ChatEntry entry in _chatEntries )
                {
                    if( entry.Channel == channel )
                        chatLines.Add(entry.Message);
                }
            }

            tbChat.Lines = chatLines.ToArray();
            tbChat.ScrollToCaret();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            // Disable stuff
            SetStatus("Signing in...");
            ShowWaitCursor(true);
            EnableSignIn(false);
            EnableCreateAccount(false);

            // Subscribe to completion event
            _ffcm.SignInComplete += new EventHandler(_ffcm_SignInComplete);

            // Do the sign in
            _ffcm.SignIn(tbEmail.Text, tbPassword.Text);
        }

        private void _ffcm_OnChatChannels(object sender, EventArgs e)
        {
            FFClient ffc = (FFClient)sender;
            EnableChat(true);
            SetupChatChannels(ffc.ChatChannels);
        }

        void _ffcm_SignInComplete(object sender, EventArgs e)
        {
            FFClient ffc = (FFClient)sender;
            if (ffc.AccountId < 0)
            {
                SetStatus("Account doesn't exist");
                EnableCreateAccount(true);
                EnableSignIn(true);
            }
            else if(ffc.DisplayName != null)
            {
                SetStatus("Signed In: " + ffc.DisplayName);
            }
            else
            {
                SetStatus("Invalid password");
                EnableSignIn(true);
            }
            ShowWaitCursor(false);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _ffcm.Destroy();
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            CreateAccountDlg dlg = new CreateAccountDlg();
            dlg.tbEmail.Text = tbEmail.Text;
            dlg.tbPw1.Text = tbPassword.Text;
            dlg.tbPw2.Text = tbPassword.Text;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.tbPw1.Text != dlg.tbPw2.Text)
                    MessageBox.Show("passwords don't match");
                else if (dlg.tbDisplayName.Text.Length <= 0)
                    MessageBox.Show("display name empty");
                else
                {
                    SetStatus("Creating Account");
                    EnableCreateAccount(false);
                    EnableSignIn(false);
                    ShowWaitCursor(true);
                    _ffcm.CreateAccount(dlg.tbEmail.Text, dlg.tbPw1.Text, dlg.tbDisplayName.Text);
                }
            }
        }

        private void tabChatTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterChat();
        }

        private void btnSendChat_Click(object sender, EventArgs e)
        {
            SendChat();
        }

        private void tbChatInput_KeyUp(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Enter )
                SendChat();
        }

        void SendChat()
        {
            string chatLine = tbChatInput.Text;
            if( chatLine.Length > 0 )
            {
                ChatChannel channel = GetChatChannel();
                _ffcm.SendChat((int)channel, chatLine);
                tbChatInput.Text = "";
            }
        }
    }
}
