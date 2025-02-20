using System.Text.Json;

namespace Client
{

    public partial class Game : Form
    {

        private List<Label> dashLabels = new List<Label>();
        private Room RoomData { get; set; }
        public int RoomID { get; set; }

        public Game(int roomID)
        {
            RoomID = roomID;
            InitializeComponent();
            label1.Text = $"Room ID: {roomID} - Game Started!";
            CreateAlphabetButtons();

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
            Button clickedButton = sender as Button;
            char clickedChar = char.ToLower(clickedButton.Text[0]);
            // clickedButton.Enabled = false;

            // trigger the guess_letter case 3nd el server
            // The data format is "roomID|guessedLetter"
            Connection.SendToServer(PlayEvents.GUESS_LETTER, $"{RoomID}|{clickedChar}");



        }


        private void DisplayDashes(string wordPlaceholder)
        {
            //panel2.Controls.Clear();
            //dashLabels.Clear();
            int x = 10, y = 10, spacing = 10;
            for (int i = 0; i < wordPlaceholder.Length; i++)
            {
                Label dashLabel = new Label();
                dashLabel.Text = wordPlaceholder[i].ToString();
                dashLabel.Font = new Font("Arial", 16, FontStyle.Bold);
                dashLabel.AutoSize = true;
                dashLabel.Location = new Point(x, y);
                panel2.Controls.Add(dashLabel);
                dashLabels.Add(dashLabel);
                x += dashLabel.Width + spacing;
            }
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
                // Check if incoming event is valid json
                if (string.IsNullOrEmpty(processedEvent.Data))
                {
                    continue;
                }
                RoomData = ConvertRoom(processedEvent.Data);

                switch (processedEvent.Event)
                {
                    case PlayEvents.PLAYER_JOINED:
                        {
                            if (RoomData.Player2 != null)
                            {
                                UpdateGameStateUI();
                                UpdateUI(RoomData.Player2.Name);
                            }
                        }
                        break;
                    case PlayEvents.SEND_ROOM_DATA:
                        {
                            if (RoomData.Player2 != null)
                            {
                                UpdateGameStateUI();
                                UpdateUI(RoomData.Watchers.Count);
                            }
                        }
                        break;
                    case PlayEvents.WATCH_ROOM:
                        {
                            UpdateGameStateUI();
                            UpdateUI(RoomData.Watchers.Count);
                        }
                        break;
                    case PlayEvents.KICK_EVERYONE:
                        {
                            MessageBox.Show("You have been kicked out of the room!");
                        }
                        break;
                    case PlayEvents.ROOM_UPDATE:
                        {
                            UpdateGameStateUI();
                            break;
                        }

                    case PlayEvents.GAME_OVER:
                        {
                            this.Invoke((MethodInvoker)delegate
                            {

                                MessageBox.Show("Game Over! Winner: " + RoomData.CurrentTurn.Name);
                                DisableAlphabetButtons();
                            });
                            break;
                        }

                    case PlayEvents.GAME_NOT_STARTED:
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show("Game has not started yet! Waiting for Player 2.");
                            });
                            break;
                        }
                    case PlayEvents.NOT_YOUR_TURN:
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show("It is not your turn!");
                            });
                            break;
                        }

                }
            }


        }
        private void UpdateGameStateUI()
        {
            this.Invoke((MethodInvoker)delegate
            {
                // kol ma el kelma gets revealed, no of dashes will decrease so i need to check over their count 

                if (dashLabels.Count != RoomData.ReveledLetters.Length)
                {
                    DisplayDashes(new string(RoomData.ReveledLetters));
                }
                else
                {
                    for (int i = 0; i < RoomData.ReveledLetters.Length; i++)
                    {
                        dashLabels[i].Text = RoomData.ReveledLetters[i].ToString();
                    }
                }
                label2.Text = "Current Turn: " + RoomData.CurrentTurn.Name;
            });
        }
        private void DisableAlphabetButtons()
        {
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Enabled = false;
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
            try
            {
                if (string.IsNullOrEmpty(RoomAsString))
                {
                    throw new ArgumentException("RoomAsString cannot be null or empty");
                }
                Room room = JsonSerializer.Deserialize<Room>(RoomAsString);
                return room;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
