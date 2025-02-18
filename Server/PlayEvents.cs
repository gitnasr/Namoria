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
    public static string SendEventWithData(PlayEvents playEvent)
    {
        return playEvent.ToString();
    }

    public static string SendEventWithData(PlayEvents playEvent, object data)
    {
        return $"{playEvent}|{data}";
    }

    public static ProcessedEvent ProcessEvent(string request)
    {
        
        if (request.Contains('|'))
        {
            string[] split = request.Split('|');

            string eventString = split[0];
            string data = split[1];

            return new ProcessedEvent
            {
                Event = (PlayEvents)Enum.Parse(typeof(PlayEvents), eventString),
                Data = data
            };
        }
        else
        {
            return new ProcessedEvent
            {
                Event = (PlayEvents)Enum.Parse(typeof(PlayEvents), request),
                Data = ""
            };
        }
    }

    internal static bool EventAsSting(PlayEvents eND)
    {
        throw new NotImplementedException();
    }
}
