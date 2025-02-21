using System.Text.Json;

namespace Client
{

    public partial class Game : Form
    {

        private List<Label> dashLabels = new List<Label>();
        private Room RoomData { get; set; }
        private int RoomID { get; set; }

        private string Message { get; set; }

        List<Button> KeyboardButtons;
        public Game(int roomID)
        {
            RoomID = roomID;
            InitializeComponent();
            label1.Text = $"Room ID: {roomID} ";
            CreateAlphabetButtons();

        }


        private void CreateAlphabetButtons()
        {
            KeyboardButtons = new List<Button>
            {
                buttonQ, buttonW, buttonE, buttonR, buttonT, buttonY, buttonU, buttonI, buttonO, buttonP,
                buttonA, buttonS, buttonD, buttonF, buttonG, buttonH, buttonJ, buttonK, buttonL,
                buttonZ, buttonX, buttonC, buttonV, buttonB, buttonN, buttonM
            };
            foreach (var button in KeyboardButtons)
            {
                button.Click += AlphabetButton_Click;
            }
        }

        private void AlphabetButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            char clickedChar = char.ToLower(clickedButton.Text[0]);

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
            //
            Thread listenForEvents = new Thread(async () => await ListenForEventsAsync());
            listenForEvents.Start();
        }
        private void UpdateUI(string message)
        {

            this.Invoke((MethodInvoker)delegate
            {

                label2.Text = $"Player 2: {message}";
                label3.Text = $"Host: {RoomData.Host.Name}";
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

            while (true)
            {

                string response = await Task.Run(() => Connection.ReadFromServer.ReadString());
                ProcessedEvent processedEvent = EventProcessor.ProcessEvent(response);

                DeserlizeData(processedEvent.Data);

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

                    case PlayEvents.WATCH_ROOM:
                        {
                            UpdateGameStateUI();

                            UpdateUI(RoomData.Watchers.Count);
                            UpdateUI(RoomData.Player2.Name);
                        }
                        break;
                    case PlayEvents.KICK_EVERYONE:
                        {
                            MessageBox.Show("You have been kicked out of the room!");
                            Application.Exit();
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
                                DisableButtons();
                            });
                            break;
                        }

                    case PlayEvents.GAME_NOT_STARTED:
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                MessageBox.Show(processedEvent.Data);
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
                        // Disable the button if the letter is already guessed
                        // Find the button with the same letter and disable it
                        foreach (Control ctrl in panel1.Controls)
                        {
                            if (ctrl is Button btn)
                            {
                                if (btn.Text[0].ToString().ToLower() == RoomData.ReveledLetters[i].ToString().ToLower())
                                {
                                    btn.Enabled = false;
                                }
                            }
                        }
                    }
                }
                if (RoomData.CurrentTurn.Name != Connection.Username)
                {
                    DisableButtons();
                }
                else
                {
                    EnableButtons();
                }
                toolStripStatusLabel1.Text = $"Current Turn: {RoomData.CurrentTurn.Name}";
            });
        }
        private void DisableButtons()
        {
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Enabled = false;
                }
            }
        }
        private void EnableButtons()
        {
            foreach (Control ctrl in panel1.Controls)
            {
                string ButtonText = ctrl.Text[0].ToString().ToLower();
                if (ctrl is Button btn && !RoomData.ReveledLetters.Contains(ButtonText[0]))
                {
                    btn.Enabled = true;
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
        private void DeserlizeData(string data)
        {
            try
            {
                Room room = JsonSerializer.Deserialize<Room>(data);
                RoomData = room;
            }
            catch (Exception)
            {

                Message = data;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
