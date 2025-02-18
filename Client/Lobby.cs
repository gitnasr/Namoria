using System.IO;
using System.Net.Sockets;
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
            var tempRoomCards = new List<RoomCard>();
            int IncomingCount = 0;

            while (true)
            {
                try
                {
                    string response = await Task.Run(() => Connection.ReadFromServer.ReadString());
                    ProcessedEvent parsedEvent = EventProcessor.ProcessEvent(response);

                    switch (parsedEvent.Event)
                    {
                        case PlayEvents.SEND_ROOM:
                            HandleRoomData(parsedEvent.Data, tempRoomCards);
                            IncomingCount++;
                            break;

                        case PlayEvents.END:
                            if (IncomingCount != CurrentCount)
                            {

                                this.Invoke((MethodInvoker)delegate
                                {
                                    RoomPanel.Controls.Clear();
                                    roomCards = tempRoomCards;
                                    foreach (var card in roomCards)
                                    {
                                        RoomPanel.Controls.Add(card);
                                    }
                                });
                                CurrentCount = IncomingCount;
                            }
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

        private void HandleRoomData(string RoomData, List<RoomCard> tempRoomCards)
        {
            try
            {
                string[] roomData = RoomData.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                if (roomData.Length == 3)
                {
                    int roomId = int.Parse(roomData[0]);
                    string host = roomData[1];
                    string roomState = roomData[2];

                    RoomCard card = new RoomCard(roomId, host, roomState, RoomPanel.Width);
                    card.JoinClicked += (s, id) => MessageBox.Show($"Joining Room {id}");
                    card.WatchClicked += (s, id) => MessageBox.Show($"Watching Room {id}");
                    tempRoomCards.Add(card);
                }
                else
                {
                    MessageBox.Show("Somthing went error while parsing");
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
