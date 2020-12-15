using System;

namespace Unity.Cloud.Collaborate.Models.Structures
{
    internal struct ErrorInfo : IErrorInfo
    {
        public ErrorInfo(int code = default, int priority = default, int behaviour = default, string message = default, string shortMessage = default, string codeString = default)
        {
            Code = code;
            Priority = (ErrorInfoPriority)priority;
            Behaviour = (ErrorInfoBehavior)behaviour;
            Message = message;
            ShortMessage = shortMessage;
            CodeString = codeString;
        }

        public int Code { get; }
        public ErrorInfoPriority Priority { get; }
        public ErrorInfoBehavior Behaviour { get; }
        public string Message { get; }
        public string ShortMessage { get; }
        public string CodeString { get; }
    }
}
