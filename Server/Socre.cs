namespace Server
{
    public class ScoreTracker
    {
        private readonly string filePath;
        private Dictionary<string, int> playerScores;

        public ScoreTracker(string filePath)
        {
            this.filePath = filePath;
            this.playerScores = new Dictionary<string, int>();
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
                    foreach (var player in players)
                    {
                        var parts = player.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            string name = parts[0];
                            string scoreStr = parts[1].Trim('"');
                            if (int.TryParse(scoreStr, out int score))
                            {
                                playerScores[name] = score;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading scores: {ex.Message}");
                playerScores.Clear();
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

            SaveScores();
            LogGameResult(winnerName, loserName);
        }

        private void SaveScores()
        {
            try
            {
                // Convert all scores to the format: "PLAYER_NAME "SCORE""
                var formattedScores = playerScores.Select(p => $"{p.Key} \"{p.Value}\"");

                // Join all scores with commas and write in a single line
                string scoreLines = string.Join(", ", formattedScores);

                File.WriteAllText(filePath, scoreLines);
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
