using System.Net.Sockets;

namespace Client
{
    class Connection
    {
        private static NetworkStream StreamFromNetwork;
        private static TcpClient client = new TcpClient("127.0.0.1", 5000);
        public static BinaryReader ReadFromServer;
        static BinaryWriter WriteToServer;
        public static string Username { get; private set; }

        public void ConnectToServer(string username)
        {
            try
            {
                Username = username;
                StreamFromNetwork = client.GetStream();
                ReadFromServer = new BinaryReader(StreamFromNetwork);
                WriteToServer = new BinaryWriter(StreamFromNetwork);
                // THIS IS FIRST EVENT THAT CAUSE THE SERVER GOES INTO LOOP on a Thread
                WriteToServer.Write(Username);



            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                {
                    MessageBox.Show("Server is not running");
                    return;
                }
                else
                    throw;
            }
        }
        public static void SendToServer(PlayEvents playEvent, object data = null)
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
