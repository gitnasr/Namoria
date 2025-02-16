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
        private static int count;
        private static Random random = new Random();
        private static readonly object countLock = new object();
        public int roomID { get; private set; }
        public int Host { get; private set; }
        public int? Player2 { get; set; }
        public string RandomWord { get; private set; }
        public List<int> Watchers { get; private set; }
        public RoomState RoomState { get; set; }
        public int? CurrentMove { get; set; }

        private static int GetNextRoomId()
        {
            lock (countLock)
            {
                return count++;
            }
        }
        public Room(int host, string category)
        {
            roomID = GetNextRoomId();
            Host = host;
            RandomWord = GenerateRandomWord(category);
            Player2 = null;
            Watchers = new List<int>();
            RoomState = RoomState.PENDING;
            CurrentMove = null;
        }

        private string GenerateRandomWord(string category)
        {
            List<string> categories = new List<string>();
            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Categories", category + ".txt");
            string[] words = File.ReadAllLines(filePath);

            if (words.Length == 0)
            {
               throw new Exception("Category file is empty.");
            }

            int index = random.Next(words.Length);
            return words[index];
        }
      
      
    }
}