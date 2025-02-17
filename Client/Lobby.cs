using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;



namespace Client
{
    public partial class Lobby : Form
    {
        string PlayerUsername;
        Connection connection = new Connection();


        public Lobby()
        {
            InitializeComponent();
            RequestUsername();

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


        private void Form1_Load(object sender, EventArgs e)
        {

            FlowLayoutPanel panel = flowLayoutPanel2;


            var rooms = new[]
            {
            new { ID = 1, Name = "NASR", Status = "Waiting" },
            new { ID = 2, Name = "NOURAN", Status = "Playing" },
            new { ID = 3, Name = "Nour", Status = "Full" },
            new { ID = 4, Name = "Nouran", Status = "Waiting" },
            new { ID = 5, Name = "Nouran", Status = "Waiting" },
            new { ID = 6, Name = "Nouran", Status = "Waiting" },
            new { ID = 7, Name = "Nouran", Status = "Waiting" },
            new { ID = 8, Name = "Nouran", Status = "Waiting" },
            new { ID = 9, Name = "Nouran", Status = "Waiting" },

            new { ID = 10, Name = "Nouran", Status = "Waiting" },
        };

            foreach (var room in rooms)
            {
                RoomCard card = new RoomCard(room.ID, room.Name, room.Status, panel.Width - 30);
                card.JoinClicked += (s, id) => MessageBox.Show($"Joining Room {id}");
                card.WatchClicked += (s, id) => MessageBox.Show($"Watching Room {id}");


                panel.Controls.Add(card);


            }

            this.Controls.Add(panel);

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
            this.Hide();

        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Lobby_Leave(object sender, EventArgs e)
        {
            Application.Exit();

        }
    }
}
