using Server;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

class GameServer
{
    static Categories categories = new Categories();
    static Dictionary<TcpClient, Client> clients = new Dictionary<TcpClient, Client>();
    static object lockObj = new object();
    static int ClientsCount = 0;
    static readonly List<Room> rooms = new List<Room>();
    static ScoreTracker scoreTracker = new ScoreTracker("scores.txt");

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 4782);
        server.Start();
        Console.WriteLine("Server started on port 4782...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Thread ClientThread = new Thread(() => HandleClient(client));
            ClientThread.Start();
        }
    }

    static void BroadCastToEveryOneInARoom(PlayEvents Command, int roomID, string data)
    {

        Room? room = rooms.Find(r => r.roomID == roomID);
        if (room != null)
        {

            Client? Host = room?.Host;
            Client? player2 = room?.Player2;
            TcpClient hostClient = clients.FirstOrDefault(c => c.Value.ID == Host?.ID).Key;
            if (hostClient != null)
            {
                NetworkStream stream = hostClient.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(EventProcessor.SendEventWithData(Command, data));
            }
            if (room?.Player2 != null)
            {

                int player2ID = room.Player2.ID;
                TcpClient player2Client = clients.FirstOrDefault(c => c.Value.ID == player2ID).Key;
                if (player2Client != null)
                {
                    NetworkStream stream = player2Client.GetStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(EventProcessor.SendEventWithData(Command, data));
                }
            }
            if (room?.Watchers.Count > 0)
            {
                foreach (Client Watcher in room.Watchers)
                {
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

    }

    static Room? GetRoomById(int RoomID)
    {
        Room? room = rooms.Find(r => r.roomID == RoomID);
        if (room == null)
        {
            return null;
        }
        return room;
    }

    static string GetRoomByIdAsJson(int RoomID)
    {
        Room? room = rooms.Find(r => r.roomID == RoomID);
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
            WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.CLIENT_ID, id));

            string? clientIpAddress = ClientConnection.Client?.RemoteEndPoint?.ToString();
            if (clientIpAddress != null)
            {
                Console.WriteLine($"{clients[ClientConnection].Name} has joined the Game. With an IP {clientIpAddress}");
            }
            else
            {
                Console.WriteLine($"{clients[ClientConnection].Name} has joined the Game. IP address could not be determined.");
            }
        }





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
                            string Categories = categories.CategoriesAsJson();
                            WriteToClient.Write(EventProcessor.SendEventWithData(PlayEvents.SEND_CATEGORIES, Categories));
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

                                Client host = room.Host;

                                room.RoomState = RoomState.UNAVAILABLE;



                                TcpClient hostClient = clients.FirstOrDefault(c => c.Value.ID == host.ID).Key;
                                if (hostClient != null)
                                {
                                    NetworkStream HostStream = hostClient.GetStream();
                                    BinaryWriter WriteToHost = new BinaryWriter(HostStream);
                                    string PlayerName = clients[ClientConnection].Name;

                                    WriteToHost.Write(EventProcessor.SendEventWithData(PlayEvents.JOIN_REQUEST, clients[ClientConnection].ID));
                                }



                            }



                        }
                        break;

                    case PlayEvents.JOIN_ACCEPTED:
                        {
                            // Data format: "roomID|playerId"

                            string[] parts = processedEvent.Data.Split('|');
                            if (parts.Length < 2)
                                break;
                            int playerId = int.Parse(parts[1]);
                            int roomID = int.Parse(parts[0]);

                            Room? room = rooms.Find(r => r.roomID == roomID);
                            if (room != null)
                            {
                                // find new player from tcp 
                                Client host = room.Host;
                                Client NewPlayer = clients.FirstOrDefault(c => c.Value.ID == playerId).Value;
                                NewPlayer.RoomID = roomID;
                                room.AddPlayer(NewPlayer);


                                // Send Specific Event to the new player to open the game form
                                TcpClient NewPlayerClient = clients.FirstOrDefault(c => c.Value.ID == NewPlayer.ID).Key;
                                if (NewPlayerClient != null)
                                {
                                    Console.WriteLine("Sending to Player 2");
                                    NetworkStream NewPlayerStream = NewPlayerClient.GetStream();
                                    BinaryWriter WriteToNewPlayer = new BinaryWriter(NewPlayerStream);
                                    WriteToNewPlayer.Write(EventProcessor.SendEventWithData(PlayEvents.START_GAME, roomID));
                                }
                                string roomData = GetRoomByIdAsJson(roomID);
                                BroadCastToEveryOneInARoom(PlayEvents.PLAYER_JOINED, roomID, roomData);
                            }

                        }
                        break;
                    case PlayEvents.DENY_ENTER:
                        {
                            // Data format: "roomID|playerId"
                            string[] parts = processedEvent.Data.Split('|');
                            if (parts.Length < 2)
                                break;
                            int playerId = int.Parse(parts[1]);
                            int roomID = int.Parse(parts[0]);
                            Room? room = rooms.Find(r => r.roomID == roomID);
                            room.RoomState = RoomState.WAITING;
                            TcpClient PlayerClient = clients.FirstOrDefault(c => c.Value.ID == playerId).Key;
                            if (PlayerClient != null)
                            {
                                NetworkStream PlayerStream = PlayerClient.GetStream();
                                BinaryWriter WriteToPlayer = new BinaryWriter(PlayerStream);
                                WriteToPlayer.Write(EventProcessor.SendEventWithData(PlayEvents.REJECTED_JOIN, "Bara Yala"));
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
                                string roomData = GetRoomByIdAsJson(roomID);
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
                                string RoomAsJson = GetRoomByIdAsJson(roomID);

                                if (room.Host.ID == clients[ClientConnection].ID)
                                {
                                    Console.WriteLine($"{clients[ClientConnection].Name} was the host of room {roomID}.");
                                    BroadCastToEveryOneInARoom(PlayEvents.KICK_EVERYONE, roomID, RoomAsJson);
                                    rooms.Remove(room);

                                }
                                else if (room.Player2 != null && room.Player2.ID == clients[ClientConnection].ID)
                                {
                                    Console.WriteLine($"{clients[ClientConnection].Name} was player 2 in room {roomID}.");
                                    BroadCastToEveryOneInARoom(PlayEvents.KICK_EVERYONE, roomID, RoomAsJson);
                                    rooms.Remove(room);


                                }
                                else
                                {
                                    Console.WriteLine($"{clients[ClientConnection].Name} was a watcher in room {roomID}.");
                                    room.RemoveWatcher(clients[ClientConnection]);
                                    BroadCastToEveryOneInARoom(PlayEvents.WATCH_ROOM, roomID, RoomAsJson);
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

                            Room? room = GetRoomById(roomID);
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

                            bool gameWon = room.IsWordRevealed();

                            // If the guess was wrong, switch turns.
                            if (!isCorrectGuess)
                                room.SwitchTurn();



                            string roomJson = GetRoomByIdAsJson(roomID);

                            BroadCastToEveryOneInARoom(PlayEvents.ROOM_UPDATE, roomID, roomJson);

                            if (gameWon)
                            {
                                scoreTracker.RecordGame(room.Host.Name, room.Player2.Name);
                                BroadCastToEveryOneInARoom(PlayEvents.GAME_OVER, roomID, roomJson);

                            }
                        }
                        break;
                    case PlayEvents.REPLAY_RESPONSE:
                        {
                            // Data format: "playerID|YES" or "playerID|NO"
                            string[] SplittedResponse = processedEvent.Data.Split('|');
                            if (SplittedResponse.Length != 2)
                                break;

                            if (!int.TryParse(SplittedResponse[0], out int ResponsePlayerID))
                                break;

                            string PlayAgain = SplittedResponse[1].ToUpper();
                            int roomID = clients[ClientConnection].RoomID;
                            Room room = rooms.FirstOrDefault(r => r.roomID == roomID);

                            if (room == null)
                                break;

                            string roomJson;

                            // **HOST CASE: If host rejects replay, close the room**
                            if (room.Host.ID == ResponsePlayerID && PlayAgain != "YES")
                            {
                                BroadCastToEveryOneInARoom(PlayEvents.KICK_EVERYONE, roomID, "Replay denied by host. Room closed.");
                                rooms.Remove(room);
                                break;
                            }

                            // **PLAYER 2 CASE: If player 2 rejects, remove player 2 and reset room**
                            if (room.Player2 != null && room.Player2.ID == ResponsePlayerID && PlayAgain != "YES")
                            {
                                room.Player2 = null;
                                room.HostAcceptedReplay = false;
                                room.Player2AcceptedReplay = false;
                                room.ResetRoom(room.Host);
                                roomJson = JsonSerializer.Serialize(room);
                                BroadCastToEveryOneInARoom(PlayEvents.ROOM_UPDATE, roomID, roomJson);
                                break;
                            }

                            // **Update who accepted**
                            if (room.Host.ID == ResponsePlayerID)
                                room.HostAcceptedReplay = PlayAgain == "YES";

                            if (room.Player2 != null && room.Player2.ID == ResponsePlayerID)
                                room.Player2AcceptedReplay = PlayAgain == "YES";

                            // if one of the players accepted, update the room and wait for the other player
                            if (room.HostAcceptedReplay || room.Player2AcceptedReplay)
                            {
                                room.RoomState = RoomState.WAITING;
                                roomJson = GetRoomByIdAsJson(roomID);
                                BroadCastToEveryOneInARoom(PlayEvents.ROOM_UPDATE, roomID, roomJson);
                            }

                            // if both players accepted, reset the room and start a new game
                            if (room.HostAcceptedReplay && room.Player2AcceptedReplay)
                            {
                                room.ResetRoom(room.CurrentTurn ?? room.Host);
                                room.RoomState = RoomState.PLAYING;

                                roomJson = GetRoomByIdAsJson(roomID);
                                BroadCastToEveryOneInARoom(PlayEvents.ROOM_UPDATE, roomID, roomJson);
                            }
                        }
                        break;

                    case PlayEvents.SWITCH_TURNS:
                        {
                            int roomID = int.Parse(processedEvent.Data);
                            Room? room = GetRoomById(roomID);
                            if (room == null)
                                break;
                            room.SwitchTurn();
                            string roomJson = GetRoomByIdAsJson(roomID);
                            BroadCastToEveryOneInARoom(PlayEvents.ROOM_UPDATE, roomID, roomJson);
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




}
