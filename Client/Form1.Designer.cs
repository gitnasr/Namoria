namespace Client
{
    partial class Form1
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
            playBtn = new Button();
            playerName = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // playBtn
            // 
            playBtn.Location = new Point(242, 272);
            playBtn.Margin = new Padding(4);
            playBtn.Name = "playBtn";
            playBtn.Size = new Size(231, 64);
            playBtn.TabIndex = 0;
            playBtn.Text = "Play";
            playBtn.UseVisualStyleBackColor = true;
            playBtn.Click += playBtn_Click;
            // 
            // playerName
            // 
            playerName.AutoSize = true;
            playerName.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            playerName.Location = new Point(179, 190);
            playerName.Margin = new Padding(4, 0, 4, 0);
            playerName.Name = "playerName";
            playerName.Size = new Size(192, 38);
            playerName.TabIndex = 1;
            playerName.Text = "Connected as ";
            // 
            // button1
            // 
            button1.Location = new Point(149, 392);
            button1.Name = "button1";
            button1.Size = new Size(262, 53);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(741, 467);
            Controls.Add(button1);
            Controls.Add(playerName);
            Controls.Add(playBtn);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button playBtn;
        private Label playerName;
        private Button button1;
    }
}
