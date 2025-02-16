public enum PlayEvents
{
    GET_CATEGORIES,
    GET_QUESTIONS,
    SEND_CATEGORIES,
    END // The end event here is crucial to know when to stop reading from the stream. (Inspired By Waki-Taki)
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
}
