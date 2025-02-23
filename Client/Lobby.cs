using System.Text.Json;



namespace Client
{
    public partial class Lobby : Form
    {
        string PlayerUsername;
        Connection connection = new Connection();

        private List<RoomCard> roomCards = new List<RoomCard>();
        private FlowLayoutPanel RoomPanel;

        private int PendingJoinRoomID = -1;




        public Lobby()
        {
            InitializeComponent();
            RequestUsername();
            RoomPanel = flowLayoutPanel2;

        }

        private void RequestUsername()
        {
            PlayerUsername = Microsoft.VisualBasic.Interaction.InputBox("Enter your name:", "Game Login", "Guest");
            if (string.IsNullOrWhiteSpace(PlayerUsername) || PlayerUsername == "Guest")
            {
                PlayerUsername = "Guest" + new Random().Next(1000, 9999);
            }
            _ = connection.ConnectToServer(PlayerUsername);



        }


        private async Task OnLoadGetRooms()
        {
            PendingJoinRoomID = -1;

            Connection.SendToServer(PlayEvents.GET_ROOMS);
            List<RoomCard> RoomCards = [];

            while (true)
            {
                try
                {
                    string response = await Task.Run(() => Connection.ReadFromServer.ReadString());
                    ProcessedEvent parsedEvent = EventProcessor.ProcessEvent(response);


                    switch (parsedEvent.Event)
                    {
                        case PlayEvents.SEND_ROOM:
                            HandleRoomData(parsedEvent.Data, RoomCards);
                            break;


                        case PlayEvents.END:
                            Invoke((MethodInvoker)delegate
                            {
                                RoomPanel.Controls.Clear();
                                roomCards = RoomCards;
                                foreach (var card in roomCards)
                                {
                                    RoomPanel.Controls.Add(card);
                                }
                            });
                            return;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error receiving rooms: {ex.Message}");
                    return;
                }
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            Connection.CheckForUpdate();

            CreateRoomButton.Text += $" as {PlayerUsername} \n You can play {Connection.ConnectionType}";
            timer1.Start();

        }

        private async void SendJoinRequest(int RoomID)
        {
            PendingJoinRoomID = RoomID;
            await Task.Delay(1000);
            timer1.Stop();
            Connection.SendToServer(PlayEvents.JOIN_ROOM, RoomID);

            try
            {
                while (true)
                {
                    string response = await Task.Run(() => Connection.ReadFromServer.ReadString());

                    ProcessedEvent parsedEvent = EventProcessor.ProcessEvent(response);

                    if (parsedEvent.Event == PlayEvents.START_GAME)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Game form = new Game(RoomID);
                            form.Show();
                            Hide();
                        }));
                        return;
                    }
                    else if (parsedEvent.Event == PlayEvents.REJECTED_JOIN)
                    {
                        MessageBox.Show("Host Rejected You :P");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error joining room: {ex.Message}");
            }
            finally
            {

                this.Invoke(new Action(() =>
                {
                    timer1.Start();
                }));
            }
        }


        private async Task JoinRoomAsync(int RoomID)
        {
            await Task.Run(() => SendJoinRequest(RoomID));

        }

        private void HandleRoomData(string RoomData, List<RoomCard> IncomingRoomList)
        {
            try
            {
                Room? Room = JsonSerializer.Deserialize<Room>(RoomData);

                if (Room != null)
                {

                    RoomCard card = new RoomCard(Room.roomID, Room.Host.Name, Room.RoomState.ToString(), RoomPanel.Width);
                    if (Room.Player2 != null)
                    {
                        card.JoinButton.Enabled = false;
                    }

                    if (Room.roomID == PendingJoinRoomID)
                    {
                        card.JoinButton.Enabled = false;
                        card.JoinButton.Text = "Joining...";
                    }
                    if (Room.RoomState == RoomState.UNAVAILABLE)
                    {
                        card.JoinButton.Enabled = false;
                        card.JoinButton.Text = "Someone else is joining";
                    }

                    card.JoinClicked += (s, id) => JoinRoomAsync(Room.roomID);
                    card.WatchClicked += (s, id) => WatchRoom(Room.roomID);
                    IncomingRoomList.Add(card);
                }
                else
                {
                    MessageBox.Show("Something went error while parsing");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing room data: {ex.Message}");
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Connection.CloseConnection();
            Application.Exit();

        }

        private void CreateRoomButton_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            CreateRoom CreateRoomForm = new CreateRoom();
            DialogResult result = CreateRoomForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                new Game(CreateRoomForm.RoomID).Show();
                CreateRoomForm.Close();
                Hide();

            }
            if (result == DialogResult.Cancel)
            {
                timer1.Start();
            }




        }



        private void Lobby_Leave(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _ = OnLoadGetRooms();
        }

        private void Lobby_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                timer1.Start();
            }
            else
            {
                timer1.Stop();
            }
        }
        private void WatchRoom(int RoomID)
        {
            Connection.SendToServer(PlayEvents.WATCH_ROOM, RoomID);
            Form form = new Game(RoomID);
            form.Show();
            Hide();
        }
    }
}
