using Client.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

public class RoomCard : Panel
{
    public int RoomID { get; private set; }

    public event EventHandler<int> JoinClicked;
    public event EventHandler<int> WatchClicked;


    private Label RoomLabelID;
    private Label HostLabel;

    public Button JoinButton { get; private set; }
    public Button WatchButton { get; private set; }
    public Label RoomStatus { get; private set; }


    private RoomState _roomState;
    public RoomState State
    {
        get => _roomState;
        set
        {
            _roomState = value;
            UpdateJoinButtonState();
        }
    }
    public RoomCard(int id, string name, string status, int ParentWidth )
    {
        this.RoomID = id;
        this.BackColor = Colors.GetColor(DefinedColors.primary);
        this.BorderStyle = BorderStyle.FixedSingle;
        this.Padding = new Padding(15);
        this.Margin = new Padding(10);
        this.Height = 100;
        this.Width = ParentWidth -  20;

         RoomLabelID = new Label
        {
            Text = $"#{id}",
            Font = new Font("Arial", 12, FontStyle.Bold),
            ForeColor = Colors.GetColor(DefinedColors.secondary),
            AutoSize = true,
            TextAlign = ContentAlignment.TopRight
        };

         HostLabel = new Label
        {
            Text = $"Host: {name}",
            Font = new Font("Arial", 12, FontStyle.Bold),
            ForeColor = Colors.GetColor(DefinedColors.info),
            AutoSize = true
        };

         RoomStatus = new Label
        {
            Text = status.ToUpper(),
            Font = new Font("Arial", 8),
            ForeColor = Colors.GetColor(DefinedColors.warning),
            AutoSize = true
        };

         JoinButton = new Button
        {
            Text = "JOIN",
            BackColor = Colors.GetColor(DefinedColors.neutral),
            ForeColor = Color.White,
            AutoSize = true,
            FlatStyle = FlatStyle.Flat
        };
        JoinButton.Click += (s, e) => JoinClicked?.Invoke(this, RoomID);

         WatchButton = new Button
        {
            Text = "WATCH",
            BackColor = Colors.GetColor(DefinedColors.accent),
            ForeColor = Color.White,
            AutoSize = true,
            FlatStyle = FlatStyle.Flat
        };
        WatchButton.Click += (s, e) => WatchClicked?.Invoke(this, RoomID);

        FlowLayoutPanel buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Bottom,
            Padding = new Padding(0),
            AutoSize = true
        };

        buttonPanel.Controls.Add(WatchButton);
        buttonPanel.Controls.Add(JoinButton);

        this.Controls.Add(RoomLabelID);
        this.Controls.Add(HostLabel);
        this.Controls.Add(RoomStatus);
        this.Controls.Add(buttonPanel);

        RoomLabelID.Location = new Point(ParentWidth - RoomLabelID.Width - 30, 10);
        HostLabel.Location = new Point(10, 10);
        RoomStatus.Location = new Point(10, 40);
        buttonPanel.Location = new Point(10, 70);
    }
    private void UpdateJoinButtonState()
    {
        if (JoinButton.InvokeRequired)
        {
            JoinButton.Invoke(new Action(() => UpdateJoinButtonState()));
            return;
        }

        JoinButton.Enabled = State == RoomState.WAITING;
    }
    
    public enum RoomState
    {
        WAITING,
        PLAYING,
        FULL
    }
}
