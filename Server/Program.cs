using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Server;
using System.Text.Json;

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

    static void BroadCastToEveryOneInARoom(PlayEvents Command, int roomID, string data)
    {
            
        Console.WriteLine("Broadcasting to everyone in the room");
        Room room = rooms.Find(r => r.roomID == roomID);
        Client Host = room.Host;
        Client player2 = room.Player2;
        TcpClient hostClient = clients.FirstOrDefault(c => c.Value.ID == Host.ID-1).Key;
        if (hostClient != null)
        {
            NetworkStream stream = hostClient.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(EventProcessor.SendEventWithData(Command, data));
        }
        Console.WriteLine("Sent to host");
        if (room.Player2 != null)
        {
            int player2ID = room.Player2.ID -1 ;
            TcpClient player2Client = clients.FirstOrDefault(c => c.Value.ID == player2ID).Key;
            if (player2Client != null)
            {
                NetworkStream stream = player2Client.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(EventProcessor.SendEventWithData(Command, data));
            }
        }
        if (room != null)
        {
            foreach (Client WatcherID in room.Watchers)
            {
                Console.WriteLine($"Sending to {WatcherID}");
                TcpClient client = clients.FirstOrDefault(c => c.Value.ID == WatcherID.ID-1).Key;
                if (client != null)
                {
                    NetworkStream stream = client.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(EventProcessor.EventAsSting(Command));
                }
            }
        }

    }

    static string GetAllRoomData(int RoomID)
    {
        // get room
        Room room = rooms.Find(r => r.roomID == RoomID);
        if (room == null)
        {
            return "";
        }
        string jsonString = JsonSerializer.Serialize(room);
        return jsonString;
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

        Console.WriteLine($"{clients[ClientConnection].Name} has joined the Game.");

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
                            Console.WriteLine($"Received CREATE_ROOM for category: {category}");
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
                                
                                // Improve this to go though all client in the lobby: Clients in the lobby where they have not joined a room, not in a room list.
                            }

                        }
                        break;

                    case PlayEvents.GET_ROOMS:
                            try
                            {
                                foreach (Room room in rooms)
                                {
                                   
                                    string roomDetails = $"{room.roomID}|{room.Host.Name}|{room.RoomState}";
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
                            else
                            {
                                Console.WriteLine($"Room {roomID} not found.");
                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_CREATED, -1));
                            }
                        }
                        break;
                    case PlayEvents.FETCH_ROOM_DATA:
                        {
                            int roomID = int.Parse(processedEvent.Data);
                            Console.WriteLine($"{clients[ClientConnection].Name} requested room data for room {roomID}.");
                            string roomData = GetAllRoomData(roomID);
                            WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.SEND_ROOM_DATA, roomData));
                        }
                        break;
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
                Console.WriteLine($"{File[0]}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Access Denied :{ex.Message}");
        }
    }
}
