namespace TestClient
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSignIn = new System.Windows.Forms.Button();
            this.btnCreateAccount = new System.Windows.Forms.Button();
            this.tabChatTabs = new System.Windows.Forms.TabControl();
            this.tabAll = new System.Windows.Forms.TabPage();
            this.tbChat = new System.Windows.Forms.TextBox();
            this.tbChatInput = new System.Windows.Forms.TextBox();
            this.btnSendChat = new System.Windows.Forms.Button();
            this.tabChatTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbEmail
            // 
            this.tbEmail.Location = new System.Drawing.Point(123, 32);
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(246, 20);
            this.tbEmail.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Email Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(123, 58);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(246, 20);
            this.tbPassword.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(205, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Offline";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnSignIn
            // 
            this.btnSignIn.Location = new System.Drawing.Point(294, 109);
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Size = new System.Drawing.Size(75, 23);
            this.btnSignIn.TabIndex = 2;
            this.btnSignIn.Text = "Sign In";
            this.btnSignIn.UseVisualStyleBackColor = true;
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            // 
            // btnCreateAccount
            // 
            this.btnCreateAccount.Location = new System.Drawing.Point(44, 109);
            this.btnCreateAccount.Name = "btnCreateAccount";
            this.btnCreateAccount.Size = new System.Drawing.Size(75, 23);
            this.btnCreateAccount.TabIndex = 4;
            this.btnCreateAccount.Text = "Create";
            this.btnCreateAccount.UseVisualStyleBackColor = true;
            this.btnCreateAccount.Visible = false;
            this.btnCreateAccount.Click += new System.EventHandler(this.btnCreateAccount_Click);
            // 
            // tabChatTabs
            // 
            this.tabChatTabs.Controls.Add(this.tabAll);
            this.tabChatTabs.Location = new System.Drawing.Point(12, 179);
            this.tabChatTabs.Name = "tabChatTabs";
            this.tabChatTabs.SelectedIndex = 0;
            this.tabChatTabs.Size = new System.Drawing.Size(380, 22);
            this.tabChatTabs.TabIndex = 5;
            this.tabChatTabs.SelectedIndexChanged += new System.EventHandler(this.tabChatTabs_SelectedIndexChanged);
            // 
            // tabAll
            // 
            this.tabAll.Location = new System.Drawing.Point(4, 22);
            this.tabAll.Name = "tabAll";
            this.tabAll.Padding = new System.Windows.Forms.Padding(3);
            this.tabAll.Size = new System.Drawing.Size(372, 0);
            this.tabAll.TabIndex = 0;
            this.tabAll.Text = "All";
            this.tabAll.UseVisualStyleBackColor = true;
            // 
            // tbChat
            // 
            this.tbChat.Location = new System.Drawing.Point(12, 200);
            this.tbChat.Multiline = true;
            this.tbChat.Name = "tbChat";
            this.tbChat.ReadOnly = true;
            this.tbChat.Size = new System.Drawing.Size(380, 157);
            this.tbChat.TabIndex = 6;
            // 
            // tbChatInput
            // 
            this.tbChatInput.Enabled = false;
            this.tbChatInput.Location = new System.Drawing.Point(12, 362);
            this.tbChatInput.Name = "tbChatInput";
            this.tbChatInput.Size = new System.Drawing.Size(318, 20);
            this.tbChatInput.TabIndex = 7;
            this.tbChatInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbChatInput_KeyUp);
            // 
            // btnSendChat
            // 
            this.btnSendChat.Enabled = false;
            this.btnSendChat.Location = new System.Drawing.Point(336, 362);
            this.btnSendChat.Name = "btnSendChat";
            this.btnSendChat.Size = new System.Drawing.Size(56, 20);
            this.btnSendChat.TabIndex = 8;
            this.btnSendChat.Text = "Send";
            this.btnSendChat.UseVisualStyleBackColor = true;
            this.btnSendChat.Click += new System.EventHandler(this.btnSendChat_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 394);
            this.Controls.Add(this.btnSendChat);
            this.Controls.Add(this.tbChatInput);
            this.Controls.Add(this.tbChat);
            this.Controls.Add(this.tabChatTabs);
            this.Controls.Add(this.btnCreateAccount);
            this.Controls.Add(this.btnSignIn);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbEmail);
            this.Name = "Form1";
            this.Text = "Test Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.tabChatTabs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbEmail;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnSignIn;
        private System.Windows.Forms.Button btnCreateAccount;
        private System.Windows.Forms.TabControl tabChatTabs;
        private System.Windows.Forms.TabPage tabAll;
        private System.Windows.Forms.TextBox tbChat;
        private System.Windows.Forms.TextBox tbChatInput;
        private System.Windows.Forms.Button btnSendChat;
    }
}

