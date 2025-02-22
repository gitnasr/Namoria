using Server;
using System.Reflection;



public class Room
{
    private static int count = 0;
    private static Random random = new Random();
    private static readonly object countLock = new object();
    public int roomID { get; set; }
    public Client Host { get; set; }
    public Client? Player2 { get; set; }
    public string RandomWord { get; set; }
    public List<Client> Watchers { get; set; }
    public RoomState RoomState { get; set; }
    public Client? CurrentTurn { get; set; }
    public char[] ReveledLetters { get; set; }

    public bool HostAcceptedReplay { get; set; }
    public bool Player2AcceptedReplay { get; set; }

    public string Category { get; set; }


    public Room()
    {
        Watchers = new List<Client>();
        ReveledLetters = Array.Empty<char>();


    }

    public void AddWatcher(Client client)
    {
        Watchers.Add(client);
    }
    private static int GetNextRoomId()
    {
        lock (countLock)
        {
            return ++count;
        }
    }
    public Room(Client host, string category)
    {
        Category = category;
        roomID = GetNextRoomId();
        Host = host;
        RandomWord = GenerateRandomWord();
        ReveledLetters = new char[RandomWord.Length];

        for (int i = 0; i < ReveledLetters.Length; i++)
        {
            ReveledLetters[i] = '_';
        }
        Player2 = null;
        Watchers = new List<Client>();
        RoomState = RoomState.WAITING;
        CurrentTurn = host;
        HostAcceptedReplay = false;
        Player2AcceptedReplay = false;


    }

    private string GenerateRandomWord()
    {
        List<string> categories = new List<string>();
        string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Categories", Category + ".txt");
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
    public void ResetRoom(Client winner)
    {
        RandomWord = GenerateRandomWord();
        ReveledLetters = new char[RandomWord.Length];
        for (int i = 0; i < ReveledLetters.Length; i++)
        {
            ReveledLetters[i] = '_';
        }
        CurrentTurn = winner;

        RoomState = RoomState.WAITING;
        HostAcceptedReplay = false;
        Player2AcceptedReplay = false;

    }
    public bool ProcessTurn(char guessedLetter, int playerID, out bool isCorrectGuess)
    {
        isCorrectGuess = false;
        if (playerID != CurrentTurn?.ID)
        {
            return false;
        }

        for (int i = 0; i < RandomWord.Length; i++)
        {
            if (char.ToLower(RandomWord[i]) == guessedLetter)
            {
                ReveledLetters[i] = RandomWord[i];
                isCorrectGuess = true;
            }
        }
        return true;
    }
    public bool IsWordRevealed()
    {
        foreach (char c in ReveledLetters)
        {
            if (c == '_')
                return false;
        }
        return true;
    }
    public void SwitchTurn()
    {

        if (CurrentTurn == Player2)
            CurrentTurn = Host;
        else
            CurrentTurn = Player2;
    }

    public void AddPlayer(Client client)
    {
        if (Player2 == null)
        {
            Player2 = client;
            RoomState = RoomState.PLAYING;
        }

    }

    public void RemovePlayer(Client client)
    {

        if (Player2 == client)
        {
            Player2 = null;
            RoomState = RoomState.END;
        }
    }

    public void RemoveWatcher(Client client)
    {
        if (Watchers.Contains(client))
        {
            Watchers.Remove(client);
        }
    }

}
