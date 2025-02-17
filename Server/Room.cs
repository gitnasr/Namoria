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
        public char[] revealedLetters { get; private set; }
        public List<int> Watchers { get; private set; }
        public RoomState RoomState { get; set; }
        public int? CurrentTurn { get; set; }

        // Constructor now takes a category instead of a random word.
        public Room(int host, string category)
        {
            roomID = count++;
            Host = host;
            RandomWord = GenerateRandomWord(category);
            for (int i = 0; i < revealedLetters.Length; i++)
            {
                revealedLetters[i] = '_';
            }
            revealedLetters = new char[RandomWord.Length];
            Player2 = null;
            Watchers = new List<int>();
            RoomState = RoomState.PENDING;
            CurrentTurn = host;
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
                    revealedLetters[i] = RandomWord[i];
                    isCorrectGuess = true;
                }
            }
            return true;
        }

        public bool isWordRevealed()
        {
            foreach (char c in revealedLetters)
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