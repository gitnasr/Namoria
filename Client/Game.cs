using System.Text.Json;

namespace Client
{

    public partial class Game : Form
    {

        private List<Label> dashLabels = new List<Label>();
        private Room RoomData { get; set; } = new Room();
        private int RoomID { get; set; }

        private int Timeout { get; set; } = 6;

        private int ReplayTimeout { get; set; } = 10;


        List<Button> KeyboardButtons = new List<Button>();
        public Game(int roomID)
        {
            RoomID = roomID;
            InitializeComponent();
            RoomIDLabel.Text = $"Room ID: {roomID} ";
            WatcherCounterLabel.Text = "Watcher Count: 0";
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

        private void AlphabetButton_Click(object? sender, EventArgs e)
        {
            Button? clickedButton = sender as Button;
            if (clickedButton == null)
            {
                return;
            }
            char clickedChar = char.ToLower(clickedButton.Text[0]);

            Connection.SendToServer(PlayEvents.GUESS_LETTER, $"{RoomID}|{clickedChar}");
        }


        private void DisplayDashes(string wordPlaceholder)
        {
            int spacing = 10;
            int x = (panel2.Width - (wordPlaceholder.Length * (20 + spacing))) / 2;
            int y = (panel2.Height - 20) / 2;
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

        private void StopPlayTimer()
        {
            TimeToGuessLabel.Visible = false;
            PlayTimeOutTimer.Stop();
        }

        private void Game_Load(object sender, EventArgs e)
        {
            Thread listenForEvents = new Thread(async () => await ListenForEventsAsync());
            listenForEvents.Start();
            TimeToGuessLabel.Visible = false;
        }
        private void UpdateUI(string message)
        {

            this.Invoke((MethodInvoker)delegate
            {

                Player2Label.Text = $"Player 2: {message}";
                HostNameLabel.Text = $"Host: {RoomData.Host.Name}";
            });
        }
        private void UpdateUI(int count)
        {
            this.Invoke((MethodInvoker)delegate
            {
                WatcherCounterLabel.Text = $"Watcher Count: {count}";
            });
        }
        private async Task ListenForEventsAsync()
        {
            try
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
                                UpdateGameStateUI();
                                UpdateUI(RoomData.Player2?.Name ?? "Waiting ...");
                            }
                            break;

                        case PlayEvents.WATCH_ROOM:
                            {
                                UpdateGameStateUI();
                                UpdateUI(RoomData.Watchers.Count);
                                UpdateUI(RoomData.Player2?.Name ?? "Waiting ...");
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
                                    HandleGameOver();
                                });
                                break;
                            }

                        case PlayEvents.GAME_NOT_STARTED:
                            {
                                MessageBox.Show(processedEvent.Data);
                                break;
                            }
                        case PlayEvents.NOT_YOUR_TURN:
                            {
                                MessageBox.Show("It is not your turn!");
                                break;
                            }


                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}");
                Application.Exit();
            }
        }
        private void UpdateGameStateUI()
        {
            if (this.IsDisposed || !this.IsHandleCreated)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { UpdateGameStateUI(); });
            }
            else
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
                if (RoomData?.CurrentTurn?.ID != Connection.ConnectionID || RoomData.RoomState == RoomState.WAITING)
                {
                    DisableButtons();
                    StopPlayTimer();
                }
                else
                {
                    PlayTimeOutTimer.Start();
                    Timeout = 5;
                    TimeToGuessLabel.Text = $"00:0{Timeout}";
                    EnableButtons();

                }
                toolStripStatusLabel1.Text = $"Current Turn: {RoomData?.CurrentTurn?.Name}";
            }
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
                Room? room = JsonSerializer.Deserialize<Room>(data);
                if (room != null)
                {
                    RoomData = room;
                }

            }
            catch (Exception)
            {

            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void ReplayTimeOut_Tick(object sender, EventArgs e)
        {
            if (!TimeToGuessLabel.Visible)
            {
                TimeToGuessLabel.Visible = true;
            }

            Timeout--;
            TimeToGuessLabel.Text = $"00:0{Timeout}";
            if (Timeout == 0)
            {
                PlayTimeOutTimer.Stop();
                Connection.SendToServer(PlayEvents.SWITCH_TURNS, RoomID);
            }
        }
        private void ResetReplayTimer()
        {
            ReplayTimeout = 10;
            ReplayTimer.Stop();
        }
        private void StartReplayTimer()
        {
            ReplayTimer.Start();
        }
        private void ReplayTimer_Tick(object sender, EventArgs e)
        {
            ReplayTimeout--;
            if (ReplayTimeout == 0)
            {
                ReplayTimer.Stop();
                MessageBox.Show("You we idle for a long time, exiting ...");

                Connection.SendToServer(PlayEvents.REPLAY_RESPONSE, $"{Connection.ConnectionID}|no");

                Application.Exit();
            }
        }
    }
    private void HandleGameOver()
        {
            DisableButtons();
            StopPlayTimer();
            StartReplayTimer();

            DialogResult result = MessageBox.Show(
                $"{(Connection.Username == RoomData?.CurrentTurn?.Name ? $"Winner! {RoomData.CurrentTurn.Name}" : "Hard Luck")} \n Wanna Play Again",
                "Game Over", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            string reply = result == DialogResult.Yes ? "YES" : "NO";
            Connection.SendToServer(PlayEvents.REPLAY_RESPONSE, $"{Connection.ConnectionID}|{reply}");

            if (reply == "NO")
            {
                Application.Exit();
            }
            else
            {
                ResetReplayTimer();
            }
        }
    }

}
