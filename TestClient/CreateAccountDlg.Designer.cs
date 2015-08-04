namespace TestClient
{
    partial class CreateAccountDlg
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
            this.lblPwd1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbEmail = new System.Windows.Forms.TextBox();
            this.tbPw1 = new System.Windows.Forms.TextBox();
            this.tbPw2 = new System.Windows.Forms.TextBox();
            this.tbDisplayName = new System.Windows.Forms.TextBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblPwd1
            // 
            this.lblPwd1.AutoSize = true;
            this.lblPwd1.Location = new System.Drawing.Point(62, 59);
            this.lblPwd1.Name = "lblPwd1";
            this.lblPwd1.Size = new System.Drawing.Size(53, 13);
            this.lblPwd1.TabIndex = 0;
            this.lblPwd1.Text = "Password";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Confirm Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Display Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(83, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Email";
            // 
            // tbEmail
            // 
            this.tbEmail.Location = new System.Drawing.Point(121, 26);
            this.tbEmail.Name = "tbEmail";
            this.tbEmail.Size = new System.Drawing.Size(244, 20);
            this.tbEmail.TabIndex = 4;
            // 
            // tbPw1
            // 
            this.tbPw1.Location = new System.Drawing.Point(121, 52);
            this.tbPw1.Name = "tbPw1";
            this.tbPw1.PasswordChar = '*';
            this.tbPw1.Size = new System.Drawing.Size(244, 20);
            this.tbPw1.TabIndex = 5;
            // 
            // tbPw2
            // 
            this.tbPw2.Location = new System.Drawing.Point(121, 78);
            this.tbPw2.Name = "tbPw2";
            this.tbPw2.PasswordChar = '*';
            this.tbPw2.Size = new System.Drawing.Size(244, 20);
            this.tbPw2.TabIndex = 6;
            // 
            // tbDisplayName
            // 
            this.tbDisplayName.Location = new System.Drawing.Point(121, 104);
            this.tbDisplayName.Name = "tbDisplayName";
            this.tbDisplayName.Size = new System.Drawing.Size(244, 20);
            this.tbDisplayName.TabIndex = 7;
            // 
            // btnCreate
            // 
            this.btnCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCreate.Location = new System.Drawing.Point(163, 148);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 8;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            // 
            // CreateAccountDlg
            // 
            this.AcceptButton = this.btnCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 186);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.tbDisplayName);
            this.Controls.Add(this.tbPw2);
            this.Controls.Add(this.tbPw1);
            this.Controls.Add(this.tbEmail);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPwd1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateAccountDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CreateAccountDlg";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPwd1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCreate;
        public System.Windows.Forms.TextBox tbEmail;
        public System.Windows.Forms.TextBox tbPw1;
        public System.Windows.Forms.TextBox tbPw2;
        public System.Windows.Forms.TextBox tbDisplayName;
    }
}