using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkCore
{
    public abstract class LogInterface
    {
        public enum LogMessageType
        {
            Normal,
            System,
            Game,
            Debug,
            Error,
            Security
        }

        protected static LogInterface _log;
        protected static bool _alwaysPrintToConsole = false;

        public LogInterface()
        {
            _log = this;
        }

        public abstract void Log(LogMessageType type, bool logToConsole, string message);

        public static void Log(string message, LogMessageType type = LogMessageType.Normal, bool logToConsole = false)
        {
            _log.Log(type, logToConsole, message);
        }

        public static bool AlwaysPrintToConsole
        {
            get { return _alwaysPrintToConsole; }
            set { _alwaysPrintToConsole = value; }
        }
    }
}
