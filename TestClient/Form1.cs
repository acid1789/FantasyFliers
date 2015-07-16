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
        FFCManager _ffcm;

        public Form1()
        {
            InitializeComponent();

            _ffcm = new FFCManager("127.0.0.1", 1255);

            tbEmail.Text = "ron@ungroundedgames.com";
            tbPassword.Text = "test";
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

        void _ffcm_SignInComplete(object sender, EventArgs e)
        {
            FFClient ffc = (FFClient)sender;
            if (ffc.SessionKey >= 0)
            {
                SetStatus("Signed In: " + ffc.SessionKey);
            }
            else if (ffc.SessionKey == -1)
            {
                SetStatus("Account doesn't exist");
                EnableCreateAccount(true);
            }
            else
            {
                SetStatus("Invalid password");                
            }
            EnableSignIn(true);
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
                }
            }
        }

    }
}
