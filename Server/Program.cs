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

