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
        // Split the request into exactly two parts: the event and the rest (data)
        string[] parts = request.Split(new char[] { '|' }, 2);
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

    //public static ProcessedEvent ProcessEvent(string request)
    //{
        
    //    if (request.Contains('|'))
    //    {
    //        string[] split = request.Split('|');

    //        string eventString = split[0];
    //        string data = split[1];

    //        return new ProcessedEvent
    //        {
    //            Event = (PlayEvents)Enum.Parse(typeof(PlayEvents), eventString),
    //            Data = data
    //        };
    //    }
    //    else
    //    {
    //        return new ProcessedEvent
    //        {
    //            Event = (PlayEvents)Enum.Parse(typeof(PlayEvents), request),
    //            Data = ""
    //        };
    //    }
    //}

    
}
