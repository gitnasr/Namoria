namespace Client
{
    partial class Game
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
            panel1 = new Panel();
            panel2 = new Panel();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            watcherCount = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Location = new Point(55, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(656, 98);
            panel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Location = new Point(41, 214);
            panel2.Name = "panel2";
            panel2.Size = new Size(747, 210);
            panel2.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(76, 173);
            label1.Name = "label1";
            label1.Size = new Size(59, 25);
            label1.TabIndex = 1;
            label1.Text = "label1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(376, 150);
            label2.Name = "label2";
            label2.Size = new Size(183, 25);
            label2.TabIndex = 2;
            label2.Text = "Waiting for a Partener";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(75, 129);
            label3.Name = "label3";
            label3.Size = new Size(59, 25);
            label3.TabIndex = 3;
            label3.Text = "label3";
            // 
            // watcherCount
            // 
            watcherCount.AutoSize = true;
            watcherCount.Location = new Point(628, 145);
            watcherCount.Name = "watcherCount";
            watcherCount.Size = new Size(59, 25);
            watcherCount.TabIndex = 4;
            watcherCount.Text = "label4";
            // 
            // button1
            // 
            button1.Location = new Point(180, 125);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 5;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Game
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(watcherCount);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(panel2);
            Controls.Add(label1);
            Controls.Add(panel1);
            Name = "Game";
            Text = "Game";
            FormClosing += Game_FormClosing;
            Load += Game_Load;
            Leave += Game_Leave;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label watcherCount;
        private Button button1;
    }
}