using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Server;
class GameServer
{
    static Dictionary<TcpClient, Client> clients = new Dictionary<TcpClient, Client>();
    static object lockObj = new object();
    static List<string> Categories = new List<string>();
    static int ClientsCount = 0;
    static List<Room> rooms = new List<Room>();

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
                    case PlayEvents.PLAYER_ENTERED_LOBBY:
                        Console.WriteLine($"{clients[ClientConnection].Name} requested categories.");
                        lock (lockObj)
                        {
                            foreach (string category in Categories)
                            {
                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.SEND_CATEGORY, category)); 
                            }
                            WriteToClient.Write(EventProcessor.EventAsSting(PlayEvents.END));

                        }
                        break;
                    // New case for creating a room.
                    case PlayEvents.CREATE_ROOM:
                        {
                            
                            string category = processedEvent.Data;

                            if (clients.ContainsKey(ClientConnection))
                            {
                                int hostId = clients[ClientConnection].ID;
                                Room newRoom = new Room(hostId, category);

                                lock (lockObj)
                                {
                                    rooms.Add(newRoom);
                                    clients[ClientConnection].RoomID = newRoom.roomID;
                                }

                                Console.WriteLine($"Room {newRoom.roomID} created by {clients[ClientConnection].Name} " +
                                                  $"with category '{category}' and random word '{newRoom.RandomWord}'.");

                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_CREATED, newRoom.ToString()));

                                lock (lockObj)
                                {
                                    rooms.Add(newRoom);
                                    clients[ClientConnection].RoomID = newRoom.roomID;
                                }

                                Console.WriteLine($"Room {newRoom.roomID} created by {clients[ClientConnection].Name} " +
                                                  $"with category '{category}' and random word '{newRoom.RandomWord}'.");

                                // Send all room data back to the client.
                                WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.ROOM_CREATED, newRoom.ToString()));
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

