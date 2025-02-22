namespace Server
{
    public class ScoreTracker
    {
        private readonly string filePath;
        private Dictionary<string, int> playerScores;
        private List<(string player1, string player2)> gamePairs;

        public ScoreTracker(string filePath)
        {
            this.filePath = filePath;
            this.playerScores = new Dictionary<string, int>();
            this.gamePairs = new List<(string, string)>();
            LoadScores();
        }

        private void LoadScores()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                    return;
                }

                string[] fileLines = File.ReadAllLines(filePath);
                foreach (var line in fileLines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var players = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (players.Length == 2)
                    {
                        var player1Parts = players[0].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var player2Parts = players[1].Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (player1Parts.Length == 2 && player2Parts.Length == 2)
                        {
                            string player1Name = player1Parts[0];
                            string player2Name = player2Parts[0];
                            string score1Str = player1Parts[1].Trim('"');
                            string score2Str = player2Parts[1].Trim('"');

                            if (int.TryParse(score1Str, out int score1) &&
                                int.TryParse(score2Str, out int score2))
                            {
                                playerScores[player1Name] = score1;
                                playerScores[player2Name] = score2;
                                gamePairs.Add((player1Name, player2Name));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading scores: {ex.Message}");
                playerScores.Clear();
                gamePairs.Clear();
            }
        }

        private bool PlayersExist(string player1, string player2)
        {
            return gamePairs.Any(pair =>
                (pair.player1 == player1 && pair.player2 == player2) ||
                (pair.player1 == player2 && pair.player2 == player1));
        }

        private void UpdateExistingPair(string player1, string player2)
        {
            for (int i = 0; i < gamePairs.Count; i++)
            {
                if ((gamePairs[i].player1 == player1 && gamePairs[i].player2 == player2) ||
                    (gamePairs[i].player1 == player2 && gamePairs[i].player2 == player1))
                {
                    gamePairs[i] = (player1, player2);
                    return;
                }
            }
        }

        public void RecordGame(string winnerName, string loserName)
        {
            // Update winner's score
            if (playerScores.ContainsKey(winnerName))
            {
                playerScores[winnerName]++;
            }
            else
            {
                playerScores[winnerName] = 1;
            }

            // Ensure loser exists in dictionary
            if (!playerScores.ContainsKey(loserName))
            {
                playerScores[loserName] = 0;
            }

            // Check if these players have played before
            if (PlayersExist(winnerName, loserName))
            {
                UpdateExistingPair(winnerName, loserName);
            }
            else
            {
                gamePairs.Add((winnerName, loserName));
            }

            SaveScores();
            LogGameResult(winnerName, loserName);
        }

        private void SaveScores()
        {
            try
            {
                var lines = new List<string>();

                foreach (var pair in gamePairs)
                {
                    string player1Score = $"{pair.player1} \"{playerScores[pair.player1]}\"";
                    string player2Score = $"{pair.player2} \"{playerScores[pair.player2]}\"";
                    lines.Add($"{player1Score}, {player2Score}");
                }

                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving scores: {ex.Message}");
            }
        }

        private void LogGameResult(string winnerName, string loserName)
        {
            Console.WriteLine($"Game result logged: {winnerName} with score {playerScores[winnerName]} " +
                             $"won against {loserName} with score {playerScores[loserName]}");
        }

        public Dictionary<string, int> GetAllScores()
        {
            return new Dictionary<string, int>(playerScores);
        }
    }
}