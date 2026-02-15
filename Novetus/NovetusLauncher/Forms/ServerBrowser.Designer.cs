
namespace NovetusLauncher
{
    partial class ServerBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerBrowser));
            this.JoinGameButton = new System.Windows.Forms.Button();
            this.MasterServerBox = new System.Windows.Forms.TextBox();
            this.MasterServerLabel = new System.Windows.Forms.Label();
            this.MasterServerRefreshButton = new System.Windows.Forms.Button();
            this.ServerListView = new System.Windows.Forms.ListView();
            this.AuthenticationBox = new System.Windows.Forms.CheckBox();
            this.AuthenticationTokenTextBox = new System.Windows.Forms.TextBox();
            this.TokenFieldCheckbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // JoinGameButton
            // 
            this.JoinGameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JoinGameButton.Location = new System.Drawing.Point(495, 8);
            this.JoinGameButton.Name = "JoinGameButton";
            this.JoinGameButton.Size = new System.Drawing.Size(75, 23);
            this.JoinGameButton.TabIndex = 0;
            this.JoinGameButton.Text = "JOIN GAME";
            this.JoinGameButton.UseVisualStyleBackColor = true;
            this.JoinGameButton.Click += new System.EventHandler(this.JoinGameButton_Click);
            // 
            // MasterServerBox
            // 
            this.MasterServerBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MasterServerBox.Location = new System.Drawing.Point(135, 10);
            this.MasterServerBox.Name = "MasterServerBox";
            this.MasterServerBox.Size = new System.Drawing.Size(323, 20);
            this.MasterServerBox.TabIndex = 1;
            this.MasterServerBox.TextChanged += new System.EventHandler(this.MasterServerBox_TextChanged);
            // 
            // MasterServerLabel
            // 
            this.MasterServerLabel.AutoSize = true;
            this.MasterServerLabel.Location = new System.Drawing.Point(12, 13);
            this.MasterServerLabel.Name = "MasterServerLabel";
            this.MasterServerLabel.Size = new System.Drawing.Size(117, 13);
            this.MasterServerLabel.TabIndex = 2;
            this.MasterServerLabel.Text = "Master Server Address:";
            // 
            // MasterServerRefreshButton
            // 
            this.MasterServerRefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MasterServerRefreshButton.AutoEllipsis = true;
            this.MasterServerRefreshButton.Image = global::NovetusLauncher.Properties.Resources.refresh;
            this.MasterServerRefreshButton.Location = new System.Drawing.Point(464, 8);
            this.MasterServerRefreshButton.Name = "MasterServerRefreshButton";
            this.MasterServerRefreshButton.Size = new System.Drawing.Size(25, 23);
            this.MasterServerRefreshButton.TabIndex = 3;
            this.MasterServerRefreshButton.UseVisualStyleBackColor = true;
            this.MasterServerRefreshButton.Click += new System.EventHandler(this.MasterServerRefreshButton_Click);
            // 
            // ServerListView
            // 
            this.ServerListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerListView.HideSelection = false;
            this.ServerListView.Location = new System.Drawing.Point(10, 67);
            this.ServerListView.Name = "ServerListView";
            this.ServerListView.Size = new System.Drawing.Size(555, 378);
            this.ServerListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ServerListView.TabIndex = 4;
            this.ServerListView.UseCompatibleStateImageBehavior = false;
            this.ServerListView.View = System.Windows.Forms.View.Details;
            this.ServerListView.SelectedIndexChanged += new System.EventHandler(this.ServerListView_SelectedIndexChanged);
            // 
            // AuthenticationBox
            // 
            this.AuthenticationBox.AutoSize = true;
            this.AuthenticationBox.Location = new System.Drawing.Point(15, 41);
            this.AuthenticationBox.Name = "AuthenticationBox";
            this.AuthenticationBox.Size = new System.Drawing.Size(94, 17);
            this.AuthenticationBox.TabIndex = 5;
            this.AuthenticationBox.Text = "Authentication";
            this.AuthenticationBox.UseVisualStyleBackColor = true;
            this.AuthenticationBox.CheckedChanged += new System.EventHandler(this.AuthenticationBox_CheckedChanged);
            // 
            // AuthenticationTokenTextBox
            // 
            this.AuthenticationTokenTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AuthenticationTokenTextBox.Enabled = false;
            this.AuthenticationTokenTextBox.Location = new System.Drawing.Point(248, 39);
            this.AuthenticationTokenTextBox.Name = "AuthenticationTokenTextBox";
            this.AuthenticationTokenTextBox.Size = new System.Drawing.Size(317, 20);
            this.AuthenticationTokenTextBox.TabIndex = 6;
            // 
            // TokenFieldCheckbox
            // 
            this.TokenFieldCheckbox.AutoSize = true;
            this.TokenFieldCheckbox.Enabled = false;
            this.TokenFieldCheckbox.Location = new System.Drawing.Point(114, 41);
            this.TokenFieldCheckbox.Name = "TokenFieldCheckbox";
            this.TokenFieldCheckbox.Size = new System.Drawing.Size(128, 17);
            this.TokenFieldCheckbox.TabIndex = 7;
            this.TokenFieldCheckbox.Text = "Token Authentication";
            this.TokenFieldCheckbox.UseVisualStyleBackColor = true;
            this.TokenFieldCheckbox.CheckedChanged += new System.EventHandler(this.TokenFieldCheckbox_CheckedChanged);
            // 
            // ServerBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(577, 456);
            this.Controls.Add(this.TokenFieldCheckbox);
            this.Controls.Add(this.AuthenticationTokenTextBox);
            this.Controls.Add(this.AuthenticationBox);
            this.Controls.Add(this.ServerListView);
            this.Controls.Add(this.MasterServerRefreshButton);
            this.Controls.Add(this.MasterServerLabel);
            this.Controls.Add(this.MasterServerBox);
            this.Controls.Add(this.JoinGameButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(593, 495);
            this.Name = "ServerBrowser";
            this.Text = "Server Browser";
            this.Load += new System.EventHandler(this.ServerBrowser_Load);
            this.Shown += new System.EventHandler(this.ServerBrowser_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button JoinGameButton;
        private System.Windows.Forms.TextBox MasterServerBox;
        private System.Windows.Forms.Label MasterServerLabel;
        private System.Windows.Forms.Button MasterServerRefreshButton;
        private System.Windows.Forms.ListView ServerListView;
        public System.Windows.Forms.CheckBox AuthenticationBox;
        public System.Windows.Forms.TextBox AuthenticationTokenTextBox;
        public System.Windows.Forms.CheckBox TokenFieldCheckbox;
    }
}