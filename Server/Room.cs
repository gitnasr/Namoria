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
        private static int count =0;
        private static Random random = new Random();
        private static readonly object countLock = new object();
        public int roomID { get; private set; }
        public int Host { get; private set; }
        public int? Player2 { get; set; }
        public string RandomWord { get; private set; }
        public List<int> Watchers { get; private set; }
        public RoomState RoomState { get; set; }
        public int? CurrentTurn { get; set; }
        public char[] ReveledLetters { get; private set; }

        private static int GetNextRoomId()
        {
            lock (countLock)
            {
                return ++count;
            }
        }
        public Room(int host, string category)
        {
            roomID = GetNextRoomId();
            Host = host;
            RandomWord = GenerateRandomWord(category);
            ReveledLetters = new char[RandomWord.Length];

            for (int i = 0; i < ReveledLetters.Length; i++)
            {
                ReveledLetters[i] = '_';
            }
            Player2 = null;
            Watchers = new List<int>();
            RoomState = RoomState.PENDING;
            CurrentTurn = host;
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
            Console.WriteLine(index);
            Console.WriteLine(words[index]);
            return words[index];
        }

        public bool ProcessTurn(char guessedLetter, int playerID, out bool isCorrectGuess)
        {
            isCorrectGuess = false;
            if (playerID != CurrentTurn)
            {
                return false;
            }

            for (int i = 0; i < RandomWord.Length; i++)
            {
                if (RandomWord[i] == guessedLetter)
                {
                    ReveledLetters[i] = RandomWord[i];
                    isCorrectGuess |= true;
                }
            }
            return true;
        }
        public bool isWordRevealed()
        {
            foreach (char c in ReveledLetters)
            {
                if (c == '_')
                    return false;
            }
            return true;
        }
        public void switchTurn()
        {
            if (Player2.HasValue)
            {
                if (CurrentTurn == Player2)
                    CurrentTurn = Host;
                else
                    CurrentTurn = Player2;
            }
        }
    }
}