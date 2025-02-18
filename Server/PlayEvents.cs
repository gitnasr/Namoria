public enum PlayEvents
{
    GET_CATEGORIES,
    SEND_CATEGORIES,
    CREATE_ROOM,
    ROOM_CREATED,
    GET_ROOMS,
    SEND_ROOM,
    END

}


public enum RoomState
{
    FULL,
    WAITING,
    PLAYING
}
public class ProcessedEvent
{
    public PlayEvents Event { get; set; }
    public string Data { get; set; }

    public override string ToString()
    {
        return $"{Event}|{Data}";
    }
}
public static class EventProcessor
{
    public static string EventAsSting(PlayEvents playEvent)
    {
        return playEvent.ToString();
    }

    public static string SendEventWithData(PlayEvents playEvent, object data)
    {
        return $"{playEvent}|{data}";
    }
    public static ProcessedEvent ProcessEvent(string request)
    {
  
        string[] parts = request.Split('|', 2);
        if (parts.Length == 2)
        {
            return new ProcessedEvent
            {
                Event = Enum.Parse<PlayEvents>(parts[0], true),
                Data = parts[1]
            };
        }
        else
        {
            return new ProcessedEvent
            {
                Event = Enum.Parse<PlayEvents>(request, true),
                Data = ""
            };
        }
    }



}
