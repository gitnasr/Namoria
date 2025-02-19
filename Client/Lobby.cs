using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;



namespace Client
{
    public partial class Lobby : Form
    {
        string PlayerUsername;
        Connection connection = new Connection();

        private List<RoomCard> roomCards = new List<RoomCard>();
        private FlowLayoutPanel RoomPanel;
        private int CurrentCount = 0;



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
            connection.ConnectToServer(PlayerUsername);



        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            connection.CloseConnection();
        }
        private async Task OnLoadGetRooms()
        {
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


            timer1.Start();


        }

        private void JoinRoom(int RoomID)
        {
            
            Connection.SendToServer(PlayEvents.JOIN_ROOM, RoomID);

            Form form = new Game(RoomID);

            form.Show();
            Hide();

        }

        private void HandleRoomData(string RoomData, List<RoomCard> IncomingRoomList)
        {
            try
            {
                Room Room = JsonSerializer.Deserialize<Room>(RoomData);

                if (Room != null )
                {
                    
                    RoomCard card = new RoomCard(Room.roomID, Room.Host.Name, Room.RoomState.ToString(), RoomPanel.Width);
                    if (Room.Player2 != null)
                    {
                        card.JoinButton.Enabled = false;
                    }
                    card.JoinClicked += (s, id) => JoinRoom(Room.roomID);
                    card.WatchClicked += (s, id) => MessageBox.Show($"Watching Room {id}");
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
            connection.CloseConnection();
            Application.Exit();

        }

        private void CreateRoomButton_Click(object sender, EventArgs e)
        {
            Form form = new CreateRoom(this);
            form.Show();
            Hide();

        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Lobby_Leave(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            OnLoadGetRooms();
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
    }
}
