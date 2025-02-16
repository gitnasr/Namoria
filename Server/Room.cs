using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Server
{
    public enum RoomState
    {
        PENDING,
        PLAYING
    }

    class Room
    {
        private static int count = 0;
        private static Random random = new Random();

        public int roomID { get; private set; }
        public int Host { get; private set; }
        public int? Player2 { get; set; }
        public string RandomWord { get; private set; }
        public List<int> Watchers { get; private set; }
        public RoomState RoomState { get; set; }
        public int? CurrentMove { get; set; }

        // Constructor now takes a category instead of a random word.
        public Room(int host, string category)
        {
            roomID = count++;
            Host = host;
            RandomWord = GenerateRandomWord(category);
            Player2 = null;
            Watchers = new List<int>();
            RoomState = RoomState.PENDING;
            CurrentMove = null;
        }

        private string GenerateRandomWord(string category)
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(exePath))
            {
                return "default";
            }

            string filePath = Path.Combine(exePath, "Categories", category + ".txt");

            if (!File.Exists(filePath))
            {
                return "default";
            }

            string[] words = File.ReadAllLines(filePath);

            if (words.Length == 0)
            {
                return "default";
            }

            int index = random.Next(words.Length);
            return words[index];
        }
        // Override ToString to return all room details.
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Room ID: {roomID}");
            sb.AppendLine($"Host: {Host}");
            sb.AppendLine($"Random Word: {RandomWord}");
            sb.AppendLine($"Room State: {RoomState}");
            sb.AppendLine($"Player2: {(Player2.HasValue ? Player2.Value.ToString() : "None")}");
            sb.AppendLine($"Watchers: {(Watchers.Count > 0 ? string.Join(", ", Watchers) : "None")}");
            sb.AppendLine($"Current Move: {(CurrentMove.HasValue ? CurrentMove.Value.ToString() : "None")}");
            return sb.ToString();
        }
    }
}
