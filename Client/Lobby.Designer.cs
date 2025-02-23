namespace Client
{
    partial class Lobby
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            flowLayoutPanel1 = new FlowLayoutPanel();
            CreateRoomButton = new Button();
            ExitButton = new Button();
            flowLayoutPanel2 = new FlowLayoutPanel();
            timer1 = new System.Windows.Forms.Timer(components);
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(CreateRoomButton);
            flowLayoutPanel1.Controls.Add(ExitButton);
            flowLayoutPanel1.Dock = DockStyle.Bottom;
            flowLayoutPanel1.Location = new Point(0, 580);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(666, 81);
            flowLayoutPanel1.TabIndex = 5;
            flowLayoutPanel1.WrapContents = false;
            // 
            // CreateRoomButton
            // 
            CreateRoomButton.BackColor = Color.FromArgb(61, 141, 122);
            CreateRoomButton.BackgroundImageLayout = ImageLayout.None;
            CreateRoomButton.FlatAppearance.BorderSize = 0;
            CreateRoomButton.FlatStyle = FlatStyle.Flat;
            CreateRoomButton.ForeColor = Color.Transparent;
            CreateRoomButton.Location = new Point(3, 3);
            CreateRoomButton.Name = "CreateRoomButton";
            CreateRoomButton.Size = new Size(477, 73);
            CreateRoomButton.TabIndex = 6;
            CreateRoomButton.Text = "Create a New Room!";
            CreateRoomButton.UseVisualStyleBackColor = false;
            CreateRoomButton.Click += CreateRoomButton_Click;
            // 
            // ExitButton
            // 
            ExitButton.BackColor = Color.FromArgb(86, 2, 31);
            ExitButton.FlatAppearance.BorderSize = 0;
            ExitButton.FlatStyle = FlatStyle.Flat;
            ExitButton.ForeColor = Color.Transparent;
            ExitButton.Location = new Point(486, 3);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new Size(172, 73);
            ExitButton.TabIndex = 6;
            ExitButton.Text = "Exit";
            ExitButton.UseVisualStyleBackColor = false;
            ExitButton.Click += ExitButton_Click;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.AutoScroll = true;
            flowLayoutPanel2.Dock = DockStyle.Top;
            flowLayoutPanel2.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel2.Location = new Point(0, 0);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Padding = new Padding(10);
            flowLayoutPanel2.Size = new Size(666, 574);
            flowLayoutPanel2.TabIndex = 6;
            flowLayoutPanel2.WrapContents = false;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // Lobby
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(247, 247, 247);
            ClientSize = new Size(666, 661);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(flowLayoutPanel1);
            ForeColor = Color.Black;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "Lobby";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Namoria | Lobby";
            Load += Form1_Load;
            Leave += Lobby_Leave;
            flowLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button CreateRoomButton;
        private Button ExitButton;
        private FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Timer timer1;
    }
}
