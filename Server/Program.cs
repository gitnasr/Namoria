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
                                int hostId = clients[ClientConnection].ID;
                                Room newRoom = new Room(hostId, category);
                                Console.WriteLine($"New room created: ID={newRoom.roomID}, Word={newRoom.RandomWord}");
                                lock (lockObj)
                                {
                                    rooms.Add(newRoom);
                                    clients[ClientConnection].RoomID = newRoom.roomID;
                                }
                                Console.WriteLine($"Room {newRoom.roomID} created by {clients[ClientConnection].Name} with category '{category}' and random word '{newRoom.RandomWord}'.");
                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_CREATED, newRoom.roomID));
                                // **Broadcast the new room data to all connected clients:**
                                string roomDetails = $"{newRoom.roomID}|{newRoom.Host}|{newRoom.RoomState}";
                                string formattedEvent = EventProcessor.SendEventWithData(PlayEvents.SEND_ROOM, roomDetails);
                                lock (lockObj)
                                {
                                    foreach (TcpClient clientt in clients.Keys)
                                    {
                                        try
                                        {
                                            BinaryWriter writer = new BinaryWriter(clientt.GetStream());
                                            writer.Write(formattedEvent);
                                            writer.Flush();
                                            Console.WriteLine($"Broadcast sent: {formattedEvent}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Broadcast error: " + ex.Message);
                                        }
                                    }
                                }
                            }

                        }
                        break;

                    case PlayEvents.GET_ROOMS:
                        Console.WriteLine($"{clients[ClientConnection].Name} requested rooms list.");
                        lock (roomsLock)
                        {
                            try
                            {
                                foreach (Room room in rooms)
                                {
                                   
                                    string roomDetails = $"{room.roomID}|{room.Host}|{room.RoomState}";
                                    string formattedEvent = EventProcessor.SendEventWithData(PlayEvents.SEND_ROOM, roomDetails);
                                    WriteToClient.Write(formattedEvent);
                                    WriteToClient.Flush();
                                    Console.WriteLine($"Sent: {formattedEvent}");
                                }
                                
                                WriteToClient.Write(EventProcessor.EventAsSting(PlayEvents.END));
                                WriteToClient.Flush();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error sending rooms: {ex.Message}");
                            }
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
