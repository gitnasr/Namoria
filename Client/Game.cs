using System.Text.Json;

namespace Client
{
    public partial class Game : Form
    {

        private string TheWord = "apple";
        private List<char> charList;
        private List<Label> dashLabels = new List<Label>();
        private Room RoomData { get; set; }
        public int RoomID { get; set; }

        public Game(int roomID)
        {
            InitializeComponent();
            charList = new List<char>(TheWord.ToCharArray());
            CreateAlphabetButtons();
            DisplayDashes();
            RoomID = roomID;

            label1.Text = $"Room ID: {RoomID} Hello YAYYYYYYYYYYYYYYYYY!";
        }



        private void CreateAlphabetButtons()
        {
            int x = 10, y = 10; // Positioning variables
            int buttonWidth = 30, buttonHeight = 30;
            int spacing = 5; // Space between buttons

            for (char c = 'A'; c <= 'Z'; c++)
            {
                Button btn = new Button();
                btn.Text = c.ToString();
                btn.Size = new Size(buttonWidth, buttonHeight);
                btn.Location = new Point(x, y);
                btn.Click += AlphabetButton_Click; // Attach event

                panel1.Controls.Add(btn);

                // Update X position for the next button
                x += buttonWidth + spacing;

                // Move to the next row if needed (e.g., after 13 buttons)
                if (x + buttonWidth > panel1.Width)
                {
                    x = 10; // Reset X position
                    y += buttonHeight + spacing; // Move to the next row
                }
            }
        }

        private void AlphabetButton_Click(object sender, EventArgs e)
        {
            Button? clickedButton = sender as Button;
            char clickedChar = char.ToLower(clickedButton.Text[0]);
            //MessageBox.Show("You clicked: " + clickedButton.Text);
            if (charList.Contains(clickedChar))
            {
                //MessageBox.Show($"'{clickedChar}' is in the word!");
                UpdateDashes(clickedChar);
                if (IsWordComplete())
                {
                    MessageBox.Show("Congratulations! You are the winner!");
                }
            }
            else
            {
                MessageBox.Show($"'{clickedChar}' is NOT in the word.");
            }
        }

        private void DisplayDashes()
        {
            //panel2.Controls.Clear();
            //dashLabels.Clear(); // Clear previous references

            int x = 10;
            int y = 10;
            int spacing = 10;

            foreach (char c in charList)
            {
                Label dashLabel = new Label();
                dashLabel.Text = "_";
                dashLabel.Font = new Font("Arial", 16, FontStyle.Bold);
                dashLabel.AutoSize = true;
                dashLabel.Location = new Point(x, y);

                panel2.Controls.Add(dashLabel);
                dashLabels.Add(dashLabel); // Store reference for updating

                x += dashLabel.Width + spacing;
            }
        }
        private void UpdateDashes(char guessedLetter)
        {
            for (int i = 0; i < charList.Count; i++)
            {
                if (charList[i] == guessedLetter)
                {
                    dashLabels[i].Text = guessedLetter.ToString(); // Reveal letter
                }
            }
        }

        private bool IsWordComplete()
        {
            // Check if all dashes have been replaced with letters
            foreach (Label lbl in dashLabels)
            {
                if (lbl.Text == "_")
                    return false;
            }
            return true; // All letters are revealed
        }



        private void Game_Load(object sender, EventArgs e)
        {
            watcherCount.Text = "Watcher Count: 0";
            Thread listenForEvents = new Thread(async () => await ListenForEventsAsync());
            listenForEvents.Start();
        }
        private void UpdateUI(string message)
        {

            this.Invoke((MethodInvoker)delegate
            {

                label2.Text = message;
            });
        }
        private void UpdateUI(int count)
        {
            this.Invoke((MethodInvoker)delegate
            {
                watcherCount.Text = $"Watcher Count: {count}";
            });
        }
        private async Task ListenForEventsAsync()
        {
            Connection.SendToServer(PlayEvents.FETCH_ROOM_DATA, RoomID);

            while (true)
            {

                string response = await Task.Run(() => Connection.ReadFromServer.ReadString());
                ProcessedEvent processedEvent = EventProcessor.ProcessEvent(response);
                RoomData = ConvertRoom(processedEvent.Data);
                switch (processedEvent.Event)
                {
                    case PlayEvents.PLAYER_JOINED:
                        {
                            if (RoomData.Player2 != null)
                            {
                                UpdateUI(RoomData.Player2.Name);
                            }
                        }
                        break;
                    case PlayEvents.SEND_ROOM_DATA:
                        {
                            if (RoomData.Player2 != null)
                            {
                                UpdateUI(RoomData.Watchers.Count);
                            }
                        }
                        break;
                    case PlayEvents.WATCH_ROOM:
                        {

                            UpdateUI(RoomData.Watchers.Count);
                        }
                        break;
                    case PlayEvents.KICK_EVERYONE:
                        {
                            MessageBox.Show("You have been kicked out of the room!");
                        }
                        break;
                }
            }


        }

        private void Game_Leave(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Game_FormClosing(object sender, FormClosingEventArgs e)
        {
            Connection.SendToServer(PlayEvents.LEAVE_ROOM, RoomID);
            Application.Exit();


        }

        private Room ConvertRoom(string RoomAsString)
        {
            Room room = JsonSerializer.Deserialize<Room>(RoomAsString);
            return room;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
