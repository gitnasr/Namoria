using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;


namespace Client
{
    public partial class Form1 : Form
    {
        string PlayerUsername;
        Connection connection = new Connection();
        private Label roomDataLabel;


        public Form1()
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
            playerName.Text += PlayerUsername;  


        }



        private async void playBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // ba5aly el network operations 3la background task.
                await Task.Run(() =>
                {

                    Connection.SendToServer(PlayEvents.PLAYER_ENTERED_LOBBY);

                   


                    List<string> categories = new List<string>();
                    while (true)
                    {
                        string response = Connection.ReadFromServer.ReadString();

                      ProcessedEvent  processed = EventProcessor.ProcessEvent(response);

                        switch (processed.Event)
                        {

                            case PlayEvents.SEND_CATEGORY:
                                categories.Add(processed.Data);
                                break;

                          


                        }

                        if (processed.Event == PlayEvents.END)
                        {
                            break;
                        }


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

            // Display each category as a radio button.
            foreach (string category in categories)
            {
                RadioButton radioButton = new RadioButton
                {
                    Text = category,
                    Location = new Point(xPosition, yPosition),
                    AutoSize = true,
                    Font = new Font("Arial", 10)
                };

                this.Controls.Add(radioButton);
                yPosition += spacing; 
            }

            
            Button createRoomButton = new Button
            {
                Text = "Create Room",
                Location = new Point(xPosition, yPosition + 10),
                AutoSize = true,
                Font = new Font("Arial", 10)
            };

            createRoomButton.Click += CreateRoomButton_Click;

            this.Controls.Add(createRoomButton);
            
            int labelY = createRoomButton.Location.Y + createRoomButton.Height + 10;
            roomDataLabel = new Label
            {
                Text = "Room data.",
                Location = new Point(xPosition, labelY),
                AutoSize = true,
                Font = new Font("Arial", 10)
            };
            this.Controls.Add(roomDataLabel);

        }
        
        private async void CreateRoomButton_Click(object sender, EventArgs e)
        {
            string selectedCategory = null;
            foreach (Control control in this.Controls)
            {
                if (control is RadioButton radioButton && radioButton.Checked)
                {
                    selectedCategory = radioButton.Text;
                    break;
                }
            }
            Connection.SendToServer(PlayEvents.CREATE_ROOM, selectedCategory);

            string response = await Task.Run(() => Connection.ReadFromServer.ReadString());
            ProcessedEvent processed = EventProcessor.ProcessEvent(response);

            // Check if the response is a ROOM_CREATED event.
            if (processed.Event == PlayEvents.ROOM_CREATED)
            {
                
                this.Invoke(new Action(() =>
                {
                    roomDataLabel.Text = $"Room Created. Room ID: {processed.Data}";
                }));
            }
        }
        

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            connection.CloseConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
