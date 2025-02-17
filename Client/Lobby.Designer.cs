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
            flowLayoutPanel1 = new FlowLayoutPanel();
            groupBox1 = new GroupBox();
            CreateRoomButton = new Button();
            groupBox2 = new GroupBox();
            ExitButton = new Button();
            flowLayoutPanel2 = new FlowLayoutPanel();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(groupBox2);
            flowLayoutPanel1.Dock = DockStyle.Bottom;
            flowLayoutPanel1.Location = new Point(0, 553);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(675, 108);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(CreateRoomButton);
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(353, 103);
            groupBox1.TabIndex = 9;
            groupBox1.TabStop = false;
            // 
            // CreateRoomButton
            // 
            CreateRoomButton.BackColor = Color.FromArgb(34, 153, 84);
            CreateRoomButton.BackgroundImageLayout = ImageLayout.None;
            CreateRoomButton.Dock = DockStyle.Bottom;
            CreateRoomButton.FlatStyle = FlatStyle.Flat;
            CreateRoomButton.ForeColor = Color.Transparent;
            CreateRoomButton.Location = new Point(3, 15);
            CreateRoomButton.Name = "CreateRoomButton";
            CreateRoomButton.Size = new Size(347, 85);
            CreateRoomButton.TabIndex = 6;
            CreateRoomButton.Text = "Create a New Room!";
            CreateRoomButton.UseVisualStyleBackColor = false;
            CreateRoomButton.Click += CreateRoomButton_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(ExitButton);
            groupBox2.Location = new Point(362, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(300, 103);
            groupBox2.TabIndex = 10;
            groupBox2.TabStop = false;
            // 
            // ExitButton
            // 
            ExitButton.BackColor = Color.FromArgb(225, 117, 100);
            ExitButton.Dock = DockStyle.Bottom;
            ExitButton.FlatStyle = FlatStyle.Flat;
            ExitButton.Location = new Point(3, 15);
            ExitButton.Name = "ExitButton";
            ExitButton.Size = new Size(294, 85);
            ExitButton.TabIndex = 6;
            ExitButton.Text = "Disconnect and Exit";
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
            flowLayoutPanel2.Size = new Size(675, 547);
            flowLayoutPanel2.TabIndex = 6;
            flowLayoutPanel2.WrapContents = false;
            flowLayoutPanel2.Paint += flowLayoutPanel2_Paint;
            // 
            // Lobby
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(9, 18, 44);
            ClientSize = new Size(675, 661);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(flowLayoutPanel1);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Margin = new Padding(4);
            Name = "Lobby";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Namoria | Lobby";
            Load += Form1_Load;
            Leave += Lobby_Leave;
            flowLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Button CreateRoomButton;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button ExitButton;
        private FlowLayoutPanel flowLayoutPanel2;
    }
}
