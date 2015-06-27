using Core;

public class TemplateParseEvent : Event
{
    public static string STATE_CHANGE = "state_change";

    public static string PROGRESS_CHANGE = "progress_change";

    public static string PARSE_ERROR = "parse_error";

    public TemplateParseEvent(string type, object data = null) : base(type, data)
    {

    }
}
