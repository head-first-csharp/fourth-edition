namespace Unity.Cloud.Collaborate.Models.Structures
{
    internal interface IErrorInfo
    {
        int Code { get; }
        ErrorInfoPriority Priority { get; }
        ErrorInfoBehavior Behaviour { get; }
        string Message { get; }
        string ShortMessage { get; }
        string CodeString { get; }
    }

    internal enum ErrorInfoPriority
    {
        Critical = 0,
        Error,
        Warning,
        Info,
        None
    }

    internal enum ErrorInfoBehavior
    {
        Alert = 0,
        Automatic,
        Hidden,
        ConsoleOnly,
        Reconnect
    }
}
