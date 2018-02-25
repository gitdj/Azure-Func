public class LogDetails
    {
        public string LogMsg;
        public DateTime LogMsgTime;
        public LogLevel Level;
    }
    public enum LogLevel
    {
        Info,
        Warn,
        Error
    }