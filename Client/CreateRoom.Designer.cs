namespace Client
{
    partial class CreateRoom
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
            BackButton = new Button();
            CreateRoomButton = new Button();
            CategoriesDropDown = new ComboBox();
            SuspendLayout();
            // 
            // BackButton
            // 
            BackButton.BackColor = Color.Maroon;
            BackButton.FlatAppearance.BorderSize = 0;
            BackButton.FlatStyle = FlatStyle.Popup;
            BackButton.ForeColor = SystemColors.Control;
            BackButton.Location = new Point(254, 50);
            BackButton.Margin = new Padding(2);
            BackButton.Name = "BackButton";
            BackButton.Size = new Size(238, 58);
            BackButton.TabIndex = 5;
            BackButton.Text = "Nvm, I Want to Join Existing Room";
            BackButton.UseVisualStyleBackColor = false;
            BackButton.Click += BackButton_Click;
            // 
            // CreateRoomButton
            // 
            CreateRoomButton.BackColor = Color.FromArgb(0, 64, 64);
            CreateRoomButton.FlatStyle = FlatStyle.Popup;
            CreateRoomButton.ForeColor = SystemColors.ButtonFace;
            CreateRoomButton.Location = new Point(12, 50);
            CreateRoomButton.Margin = new Padding(2);
            CreateRoomButton.Name = "CreateRoomButton";
            CreateRoomButton.Size = new Size(238, 58);
            CreateRoomButton.TabIndex = 6;
            CreateRoomButton.Text = "Let's Get this party started!";
            CreateRoomButton.UseVisualStyleBackColor = false;
            CreateRoomButton.Click += button1_Click_1Async;
            // 
            // CategoriesDropDown
            // 
            CategoriesDropDown.FormattingEnabled = true;
            CategoriesDropDown.Location = new Point(12, 12);
            CategoriesDropDown.Name = "CategoriesDropDown";
            CategoriesDropDown.Size = new Size(482, 33);
            CategoriesDropDown.TabIndex = 7;
            // 
            // CreateRoom
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(247, 247, 247);
            ClientSize = new Size(505, 124);
            Controls.Add(CategoriesDropDown);
            Controls.Add(BackButton);
            Controls.Add(CreateRoomButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2);
            MaximizeBox = false;
            Name = "CreateRoom";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Create Room";
            FormClosed += CreateRoom_FormClosed;
            Load += CreateRoom_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button BackButton;
        private Button CreateRoomButton;
        private ComboBox CategoriesDropDown;
    }
}