using Server;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;

class GameServer
{
    public static List<string> Categories = new List<string>();

    static Dictionary<TcpClient, Client> clients = new Dictionary<TcpClient, Client>();
    static object lockObj = new object();
    static int ClientsCount = 0;
    static readonly List<Room> rooms = new List<Room>();

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 4782);
        server.Start();
        Console.WriteLine("Server started on port 4782...");
        FetchFiles();
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread ClientThread = new Thread(() => HandleClient(client));
            ClientThread.Start();
        }
    }

    static void BroadCastToEveryOneInARoom(PlayEvents Command, int roomID, string data)
    {

        Console.WriteLine($"Broadcasting {Command} to everyone in the room");
        Room? room = rooms.Find(r => r.roomID == roomID);
        if (room != null)
        {

            Client? Host = room?.Host;
            Client? player2 = room?.Player2;
            TcpClient hostClient = clients.FirstOrDefault(c => c.Value.ID == Host?.ID).Key;
            if (hostClient != null)
            {
                Console.WriteLine($"Sending to Host {Host.Name} @ id {Host.ID}");
                NetworkStream stream = hostClient.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(EventProcessor.SendEventWithData(Command, data));
            }
            if (room?.Player2 != null)
            {

                int player2ID = room.Player2.ID;
                Console.WriteLine($"Sending to Player 2 {room.Player2.Name} @ id {player2ID}");

                TcpClient player2Client = clients.FirstOrDefault(c => c.Value.ID == player2ID).Key;
                if (player2Client != null)
                {
                    NetworkStream stream = player2Client.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(EventProcessor.SendEventWithData(Command, data));
                }
            }

            foreach (Client Watcher in room.Watchers)
            {
                Console.WriteLine($"Sending to Watchers {Watcher.Name}");
                TcpClient client = clients.FirstOrDefault(c => c.Value.ID == Watcher.ID).Key;
                if (client != null)
                {
                    NetworkStream stream = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(EventProcessor.SendEventWithData(Command, data));
                }
            }


        }

    }



    static string GetAllRoomData(int RoomID)
    {

        Room room = rooms.Find(r => r.roomID == RoomID);
        if (room == null)
        {
            return "";
        }
        return JsonSerializer.Serialize(room);
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
        // Get an ip address of the client from tcpclient
        string clientIpAddress = ClientConnection.Client.RemoteEndPoint.ToString();
        Console.WriteLine($"{clients[ClientConnection].Name} has joined the Game. With an IP {clientIpAddress}");


        try
        {
            while (true)
            {
                string request = reader.ReadString();
                ProcessedEvent processedEvent = EventProcessor.ProcessEvent(request);
                switch (processedEvent.Event)
                {
                    case PlayEvents.GET_CATEGORIES:
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
                            if (clients.ContainsKey(ClientConnection))
                            {
                                Client host = clients[ClientConnection];
                                Room newRoom = new Room(host, category);
                                Console.WriteLine($"New room created: ID={newRoom.roomID}, Word={newRoom.RandomWord}");
                                lock (lockObj)
                                {
                                    rooms.Add(newRoom);
                                    clients[ClientConnection].RoomID = newRoom.roomID;
                                }
                                Console.WriteLine($"Room {newRoom.roomID} created by {clients[ClientConnection].Name} with category '{category}' and random word '{newRoom.RandomWord}'.");
                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_CREATED, newRoom.roomID));

                            }

                        }
                        break;

                    case PlayEvents.GET_ROOMS:
                        try
                        {
                            foreach (Room room in rooms)
                            {

                                string roomDetails = JsonSerializer.Serialize(room);

                                string formattedEvent = EventProcessor.SendEventWithData(PlayEvents.SEND_ROOM, roomDetails);
                                WriteToClient.Write(formattedEvent);
                                WriteToClient.Flush();
                            }

                            WriteToClient.Write(EventProcessor.EventAsSting(PlayEvents.END));
                            WriteToClient.Flush();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error sending rooms: {ex.Message}");
                        }

                        break;
                    case PlayEvents.JOIN_ROOM:
                        {
                            int roomID = int.Parse(processedEvent.Data);
                            Console.WriteLine($"{clients[ClientConnection].Name} requested to join room {roomID}.");
                            Room? room = rooms.Find(r => r.roomID == roomID);
                            if (room != null)
                            {

                                Console.WriteLine($"{clients[ClientConnection].Name} joined room {roomID}.");
                                Client NewPlayer = clients[ClientConnection];
                                room.AddPlayer(NewPlayer);
                                string roomData = GetAllRoomData(roomID);

                                BroadCastToEveryOneInARoom(PlayEvents.PLAYER_JOINED, roomID, roomData);

                            }


                        }
                        break;


                    case PlayEvents.WATCH_ROOM:
                        {
                            int roomID = int.Parse(processedEvent.Data);
                            Console.WriteLine($"{clients[ClientConnection].Name} requested to watch room {roomID}.");
                            Room? room = rooms.Find(r => r.roomID == roomID);
                            if (room != null)
                            {
                                Console.WriteLine($"{clients[ClientConnection].Name} is watching room {roomID}.");
                                Client Watcher = clients[ClientConnection];
                                room.AddWatcher(Watcher);
                                string roomData = GetAllRoomData(roomID);
                                BroadCastToEveryOneInARoom(PlayEvents.WATCH_ROOM, roomID, roomData);
                            }

                        }
                        break;
                    case PlayEvents.LEAVE_ROOM:
                        {
                            int roomID = int.Parse(processedEvent.Data);
                            Console.WriteLine($"{clients[ClientConnection].Name} left room {roomID}.");
                            Room? room = rooms.Find(r => r.roomID == roomID);
                            if (room != null)
                            {
                                string roomData = GetAllRoomData(roomID);

                                if (room.Host.ID == clients[ClientConnection].ID)
                                {
                                    Console.WriteLine($"{clients[ClientConnection].Name} was the host of room {roomID}.");
                                    BroadCastToEveryOneInARoom(PlayEvents.KICK_EVERYONE, roomID, roomData);
                                    rooms.Remove(room);

                                }
                                else if (room.Player2 != null && room.Player2.ID == clients[ClientConnection].ID)
                                {
                                    Console.WriteLine($"{clients[ClientConnection].Name} was player 2 in room {roomID}.");
                                    BroadCastToEveryOneInARoom(PlayEvents.KICK_EVERYONE, roomID, roomData);
                                    rooms.Remove(room);


                                }
                                else
                                {
                                    Console.WriteLine($"{clients[ClientConnection].Name} was a watcher in room {roomID}.");
                                    room.RemoveWatcher(clients[ClientConnection]);
                                    BroadCastToEveryOneInARoom(PlayEvents.WATCH_ROOM, roomID, roomData);
                                }

                            }
                        }
                        break;
                    case PlayEvents.GUESS_LETTER:
                        {
                            // Data format: "roomID|guessedLetter"
                            string[] parts = processedEvent.Data.Split('|');
                            if (parts.Length < 2)
                                break;

                            int roomID = int.Parse(parts[0]);
                            char guessedLetter = char.ToLower(parts[1][0]);

                            Room room = rooms.Find(r => r.roomID == roomID);
                            if (room == null)
                                break;

                            // Prevent guesses before the game starts (i.e. before Player2 joins)
                            if (room.RoomState != RoomState.PLAYING)
                            {
                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.GAME_NOT_STARTED, "How about waiting for someone to start the game?"));
                                break;
                            }

                            Client currentClient = clients[ClientConnection];

                            // Check if it's the current player's turn
                            if (room.CurrentTurn.ID != currentClient.ID)
                            {
                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.NOT_YOUR_TURN, "Nice Try, But we handle this case :D !"));
                                break;
                            }

                            bool isCorrectGuess;
                            bool validTurn = room.ProcessTurn(guessedLetter, currentClient.ID, out isCorrectGuess);
                            if (!validTurn)
                                break;

                            bool gameWon = room.isWordRevealed();

                            // If the guess was wrong, switch turns.
                            if (!isCorrectGuess)
                                room.switchTurn();



                            string roomJson = JsonSerializer.Serialize(room);

                            BroadCastToEveryOneInARoom(PlayEvents.ROOM_UPDATE, roomID, roomJson);

                            if (gameWon)
                            {
                                BroadCastToEveryOneInARoom(PlayEvents.GAME_OVER, roomID, roomJson);
                                // Function (Name, Loser)
                                // Logs.txt, "PLAYERNAME "1", PLAYER2 "0" "//Save file

                            }
                            break;
                        }

                }
            }
        }
        catch
        {
            lock (lockObj)
            {
                Console.WriteLine($"{clients[ClientConnection].Name} disconnected.");


                clients.Remove(ClientConnection);
            }
        }
        finally
        {
            stream.Close();
            ClientConnection.Close();
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
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Access Denied :{ex.Message}");
        }
    }

}
