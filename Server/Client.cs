namespace Server
{
     class Client
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int RoomID { get; set; } = -1;
        public BinaryWriter WriteToClient { get; set; }  

        public Client() 
        {
            ID = -1;
            Name = "";
            RoomID = -1;
        }
        public Client(int id, string name, BinaryWriter writeToClient)
        {
            ID = id;
            Name = name;
            WriteToClient = writeToClient;
        }
    }

}
