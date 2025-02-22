namespace Server
{
    public class ScoreTracker
    {
        private readonly string filePath;
        private Dictionary<string, int> playerScores;
        private HashSet<(string, string)> gamePairs;

        public ScoreTracker(string filePath)
        {
            this.filePath = filePath;
            this.playerScores = new Dictionary<string, int>();
            this.gamePairs = new HashSet<(string, string)>();
            LoadScores();
        }

        private void LoadScores()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, string.Empty);
                    return;
                }

                string[] fileLines = File.ReadAllLines(filePath);
                foreach (var line in fileLines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var players = line.Split(',');
                    if (players.Length != 2) continue;

                    var player1Data = players[0].Trim().Split(' ');
                    var player2Data = players[1].Trim().Split(' ');

                    if (player1Data.Length < 2 || player2Data.Length < 2) continue;

                    string player1 = player1Data[0];
                    string player2 = player2Data[0];
                    string score1Str = player1Data[1].Trim('"');
                    string score2Str = player2Data[1].Trim('"');

                    if (int.TryParse(score1Str, out int score1) && int.TryParse(score2Str, out int score2))
                    {
                        playerScores[player1] = score1;
                        playerScores[player2] = score2;
                        gamePairs.Add((player1, player2));
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

        public void RecordGame(string winner, string loser)
        {
            if (playerScores.ContainsKey(winner))
                playerScores[winner]++;
            else
                playerScores[winner] = 1;

            if (!playerScores.ContainsKey(loser))
                playerScores[loser] = 0;

            gamePairs.Add((winner, loser));

            SaveScores();
            Console.WriteLine($"Game recorded: {winner} won against {loser}");
        }

        private void SaveScores()
        {
            try
            {
                var lines = new List<string>();

                foreach (var pair in gamePairs)
                {
                    if (!playerScores.ContainsKey(pair.Item1) || !playerScores.ContainsKey(pair.Item2))
                        continue;

                    string player1Score = $"{pair.Item1} \"{playerScores[pair.Item1]}\"";
                    string player2Score = $"{pair.Item2} \"{playerScores[pair.Item2]}\"";
                    lines.Add($"{player1Score}, {player2Score}");
                }

                File.WriteAllLines(filePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving scores: {ex.Message}");
            }
        }

        public Dictionary<string, int> GetAllScores()
        {
            return new Dictionary<string, int>(playerScores);
        }
    }
}
