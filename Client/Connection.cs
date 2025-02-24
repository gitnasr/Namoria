using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Client
{
    class Connection
    {
        public static string ConnectionType { get; private set; } = "";
        public static string Username { get; private set; } = "";
        public static int ConnectionID { get; private set; }

      

        private readonly static string DDNS_ADDRESS = "gitnasr.ddns.net";
        private readonly static string FALLBACK_ADDRESS = "127.0.0.1";
        private static NetworkStream StreamFromNetwork = default!;
        private static TcpClient client = new TcpClient(GetConnectionAddress(), 4782);
        public static BinaryReader ReadFromServer = default!;
        static BinaryWriter WriteToServer = default!;

        static string GetConnectionAddress()
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(DDNS_ADDRESS, 2000); // 2 second timeout, we won't need our users waiting for long :D 
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    ConnectionType = "Over the internet";
                    return DDNS_ADDRESS;
                }
            }
            catch (PingException)
            {
                // Failed to Connect to the DDNS server, we will fallback to the local server
            }

            ConnectionType = "Only through this pc";
            return FALLBACK_ADDRESS;
        }
        public async Task ConnectToServer(string username)
        {

            try
            {
                Username = username;
                StreamFromNetwork = client.GetStream();
                ReadFromServer = new BinaryReader(StreamFromNetwork);
                WriteToServer = new BinaryWriter(StreamFromNetwork);
                // THIS IS FIRST EVENT THAT CAUSE THE SERVER GOES INTO LOOP on a Thread
                WriteToServer.Write(Username);

                ProcessedEvent EventWithID = new ProcessedEvent();
                EventWithID = EventProcessor.ProcessEvent(await Task.Run(() => ReadFromServer.ReadString()));
                ConnectionID = int.Parse(EventWithID.Data);


            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                {
                    MessageBox.Show("Server is not running");
                    Application.Exit();
                    return;
                }
                else
                    throw;
            }
        }
        public static void SendToServer(PlayEvents playEvent, object? data = null)
        {
            if (data == null)
                WriteToServer.Write(EventProcessor.EventAsSting(playEvent));
            else
                WriteToServer.Write(EventProcessor.SendEventWithData(playEvent, data));
        }

        public static void CloseConnection()
        {
            client.Close();
            ReadFromServer.Close();
            WriteToServer.Close();
        }
    }
}
