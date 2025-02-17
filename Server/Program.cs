using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Server;
class GameServer
{
    public static List<string> Categories = new List<string>();

    static Dictionary<TcpClient, Client> clients = new Dictionary<TcpClient, Client>();
    static object lockObj = new object();
    static int ClientsCount = 0;
    static readonly List<Room> rooms = new List<Room>(); 
    static readonly object roomsLock = new object();
    static void Main()
    {
        
        TcpListener server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("Server started on port 5000...");
        Console.WriteLine("Available Categories:");
        FetchFiles();
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread ClientThread = new Thread(() => HandleClient(client));
            ClientThread.Start();
        }
    }

    static void HandleClient(TcpClient ClientConnection)
    {
       
        NetworkStream stream = ClientConnection.GetStream();
        BinaryReader reader = new BinaryReader(stream);
        BinaryWriter WriteToClient = new BinaryWriter(stream);
        Client client;

        string guestUsername = reader.ReadString();

        lock (lockObj)
        {
            int id = ++ClientsCount;
            client = new Client(id, guestUsername);
            clients[ClientConnection] = client;
           
        }

        Console.WriteLine(clients[ClientConnection].Name + " has joined the Game.");

        try
        {
            while (true)
            {
                string request = reader.ReadString();
                ProcessedEvent processedEvent = EventProcessor.ProcessEvent(request);
                switch (processedEvent.Event)
                {
                    case PlayEvents.GET_CATEGORIES:
                        Console.WriteLine($"{clients[ClientConnection].Name} requested categories.");
                        lock (lockObj)
                        {
                            foreach (string category in Categories)
                            {
                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.SEND_CATEGORIES, category));
                            }
                            WriteToClient.Write(EventProcessor.EventAsSting(PlayEvents.END));

                        }
                        break;
                    case PlayEvents.CREATE_ROOM:
                        {

                            string category = processedEvent.Data;
                            Console.WriteLine(category);
                            if (clients.ContainsKey(ClientConnection))
                            {
                                int hostId = clients[ClientConnection].ID;
                                Room newRoom = new Room(hostId, category);
                                Console.WriteLine(newRoom.roomID);
                                Console.WriteLine(newRoom.RandomWord);
                                lock (lockObj)
                                {
                                    rooms.Add(newRoom);
                                    clients[ClientConnection].RoomID = newRoom.roomID;
                                }

                                Console.WriteLine($"Room {newRoom.roomID} created by {clients[ClientConnection].Name} " +
                                                  $"with category '{category}' and random word '{newRoom.RandomWord}'.");

                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_CREATED, newRoom.roomID));

                              
                            }
                            break;
                        }
                    case PlayEvents.PLAYER_TURN:

                        // Expected format for Data: "roomID,guessedLetter"
                        string[] moveParts = processedEvent.Data.Split(',');
                        if (moveParts.Length < 2)
                        {
                            WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_UPDATE, "Invalid turn format."));
                            break;
                        }

                        if (!int.TryParse(moveParts[0], out int roomId))
                        {
                            WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_UPDATE, "Invalid room ID."));
                            break;
                        }
                        char guessedLetter = moveParts[1][0];

                        // Look up the room by its ID.
                        Room room = null;
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            if (rooms[i].roomID == roomId)
                            {
                                room = rooms[i];
                                break;
                            }
                        }
                        if (room == null)
                        {
                            WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_UPDATE, "Room NOT FOUND."));
                            break;
                        }

                        // Process the current turn using the room's ProcessTurn method
                        bool validMove = room.ProcessTurn(guessedLetter, clients[ClientConnection].ID, out bool revealedLetter);
                        if (!validMove)
                        {
                            WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_UPDATE, "Not your turn."));
                            break;
                        }

                        // Check if there is a winner
                        if (room.isWordRevealed())
                        {
                            //
                        }
                        else
                        {  // Toggle Turns
                            room.switchTurn();
                            string revealedLetterAsString = new string(room.revealedLetters);
                            string updateStateMsg = EventProcessor.SendEventWithData(PlayEvents.UPDATE_GAME_STATE, $"{revealedLetterAsString}|{room.CurrentTurn}");
                            BroadcastToRoom(room, updateStateMsg);
                        }
                        break;
                }
            }

        }
        catch
        {
            lock (lockObj)
            {
                Console.WriteLine(clients[ClientConnection].Name + " disconnected.");
                clients.Remove(ClientConnection);
            }

        }
        finally
        {
            stream.Close();
            ClientConnection.Close();
        }
    }

    static void BroadcastToRoom(Room room, string message)
    {
        lock (lockObj)
        {
            foreach (KeyValuePair<TcpClient, Client> dict in clients)
            {
                Client client = dict.Value;

                if (client.RoomID == room.roomID)
                {
                    try
                    {
                        client.WriteToClient.Write(message);
                        client.WriteToClient.Flush();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error broadcasting to client {client.ID}: {ex.Message}");
                    }
                }
            }
        }
    }

    static void FetchFiles()
    {
        string? ExEPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (ExEPath == null)
        {
            Console.WriteLine("Path is null");
            return;
        }
        string path = Path.Combine(ExEPath, "Categories");
        try
        {
            foreach (string file in Directory.GetFiles(path))
            {
                string[] File = Path.GetFileName(file).Split('.');
                Categories.Add(File[0]);
                Console.WriteLine($"{File[0]}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Access Denied :{ex.Message}");
        }
    }
    
}

