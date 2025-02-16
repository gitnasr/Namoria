using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;


namespace Client
{
    public partial class Form1 : Form
    {
        string PlayerUsername;
        Connection connection = new Connection();
 

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

                yPosition += spacing; // 3shan yeego t7t b3d
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
