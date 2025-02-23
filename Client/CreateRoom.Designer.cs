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
            CategoriesTable = new TableLayoutPanel();
            BackButton = new Button();
            CreateRoomButton = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // CategoriesTable
            // 
            CategoriesTable.ColumnCount = 2;
            CategoriesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            CategoriesTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            CategoriesTable.Dock = DockStyle.Top;
            CategoriesTable.Location = new Point(0, 0);
            CategoriesTable.Margin = new Padding(2);
            CategoriesTable.Name = "CategoriesTable";
            CategoriesTable.Padding = new Padding(10, 10, 10, 10);
            CategoriesTable.RowCount = 2;
            CategoriesTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            CategoriesTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            CategoriesTable.Size = new Size(552, 150);
            CategoriesTable.TabIndex = 3;
            // 
            // BackButton
            // 
            BackButton.BackColor = Color.DarkSlateGray;
            BackButton.FlatAppearance.BorderSize = 0;
            BackButton.FlatStyle = FlatStyle.Flat;
            BackButton.ForeColor = SystemColors.Control;
            BackButton.Location = new Point(278, 2);
            BackButton.Margin = new Padding(2);
            BackButton.Name = "BackButton";
            BackButton.Size = new Size(271, 162);
            BackButton.TabIndex = 5;
            BackButton.Text = "Nvm, I Want to Join Existing Room";
            BackButton.UseVisualStyleBackColor = false;
            BackButton.Click += BackButton_Click;
            // 
            // CreateRoomButton
            // 
            CreateRoomButton.BackColor = Color.Indigo;
            CreateRoomButton.FlatAppearance.BorderSize = 0;
            CreateRoomButton.FlatStyle = FlatStyle.Flat;
            CreateRoomButton.ForeColor = SystemColors.InactiveCaption;
            CreateRoomButton.Location = new Point(2, 2);
            CreateRoomButton.Margin = new Padding(2);
            CreateRoomButton.Name = "CreateRoomButton";
            CreateRoomButton.Size = new Size(270, 162);
            CreateRoomButton.TabIndex = 6;
            CreateRoomButton.Text = "Let's Get this party started!";
            CreateRoomButton.UseVisualStyleBackColor = false;
            CreateRoomButton.Click += button1_Click_1Async;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(CreateRoomButton, 0, 0);
            tableLayoutPanel2.Controls.Add(BackButton, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(0, 150);
            tableLayoutPanel2.Margin = new Padding(2);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(552, 169);
            tableLayoutPanel2.TabIndex = 7;
            // 
            // CreateRoom
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(247, 247, 247);
            ClientSize = new Size(552, 319);
            Controls.Add(tableLayoutPanel2);
            Controls.Add(CategoriesTable);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2);
            Name = "CreateRoom";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Create Room";
            FormClosed += CreateRoom_FormClosed;
            Load += CreateRoom_Load;
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TableLayoutPanel CategoriesTable;
        private Button BackButton;
        private Button CreateRoomButton;
        private TableLayoutPanel tableLayoutPanel2;
    }
}