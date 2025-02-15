using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;


namespace Client
{
    public partial class Form1 : Form
    {
        string PlayerUsername;
        TcpClient client;
        NetworkStream stream;
        BinaryReader reader;
        BinaryWriter writer;
        //Thread receiveThread;
        //List<string> receivedCategories;

        public Form1()
        {
            InitializeComponent();
            RequestUsername();
            ConnectToServer();

        }

        private void RequestUsername()
        {
            PlayerUsername = Microsoft.VisualBasic.Interaction.InputBox("Enter your name:", "Game Login", "Guest");
            if (string.IsNullOrWhiteSpace(PlayerUsername) || PlayerUsername == "Guest")
            {
                PlayerUsername = "Guest" + new Random().Next(1000, 9999);
            }
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
                stream = client.GetStream();
                reader = new BinaryReader(stream);
                writer = new BinaryWriter(stream);

                // Send the username first.
                writer.Write(PlayerUsername);

                playerName.Text = "Connected as " + PlayerUsername;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message);
            }
        }

        private async void playBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // ba5aly el network operations 3la background task.
                await Task.Run(() =>
                {

                    writer.Write(EventProcessor.EventAsSting(PlayEvents.PLAYER_ENTERED_LOBBY));
                    int categoryCount = reader.ReadInt32();
                    List<string> categories = new List<string>();

                    for (int i = 0; i < categoryCount; i++)
                    {
                        categories.Add(reader.ReadString());
                    }

                    // Once received, update the GUI on the main thread.
                    this.Invoke(new Action(() =>
                    {
                        DisplayCategories(categories);
                    }));
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending message: " + ex.Message);
            }
        }

        private void DisplayCategories(List<string> categories)
        {
            this.Controls.Clear();
            int yPosition = 20;
            int xPosition = 20;
            int spacing = 30;

            foreach (string category in categories)
            {
                RadioButton radioButton = new RadioButton
                {
                    Text = category,
                    Location = new Point(xPosition, yPosition),
                    AutoSize = true,
                    Font = new Font("Arial", 10)
                };

                radioButton.Click += new EventHandler(CategorySelected);



                this.Controls.Add(radioButton);

                yPosition += spacing; // 3shan yeego t7t b3d
            }
        }

        private async void CategorySelected(object sender, EventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                await Task.Run(() =>
                {
                    writer.Write(EventProcessor.SendEventWithData(
                        PlayEvents.CATEGORY_SELECTED, radioButton.Text));
                });
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            writer?.Close();
            reader?.Close();
            stream?.Close();
            client?.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            writer.Write(EventProcessor.EventAsSting(PlayEvents.TEST_EVENT));
        }
    }
}
