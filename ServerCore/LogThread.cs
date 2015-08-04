using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace ServerCore
{
    public static class LogThread
    {
        public enum LogMessageType
        {
            Normal,
            System,
            Game,
            Debug,
            Error
        }

        class LogEntry
        {
            LogMessageType _type;
            string _text;
            bool _logToConsole;
            DateTime _timestamp;

            public LogEntry(string text, LogMessageType type, bool logToConsole)
            {
                _type = type;
                _text = text;
                _logToConsole = logToConsole;
                _timestamp = DateTime.Now;
            }

            public string GetText()
            {
                string text = _timestamp.ToString() + "," + _type.ToString() + "," + _text;
                return text;
            }

            public bool LogToConsole
            {
                get { return _logToConsole; }
            }
        }

        static Thread _logThread;
        static List<LogEntry> _entries;
        static Mutex _entriesLock;

        static bool _alwaysPrintToConsole = false;

        public static void Initialize()
        {
            if (_logThread == null)
            {
                _entries = new List<LogEntry>();
                _entriesLock = new Mutex();

                if (!Directory.Exists("./Logs"))
                    Directory.CreateDirectory("./Logs");

                _logThread = new Thread(new ThreadStart(LogThreadFunc));
                _logThread.Name = "Log Thread";
                _logThread.Start();
            }
        }

        public static void Shutdown()
        {
            if (_logThread != null)
            {
                _logThread.Abort();
                _logThread = null;

                _entriesLock.WaitOne();
                _entries.Clear();
                _entries = null;
                _entriesLock.ReleaseMutex();
                _entriesLock.Dispose();
            }
        }

        public static void Log(string message, LogMessageType type = LogMessageType.Normal, bool logToConsole = false)
        {
            if (_entries != null)
            {
                LogEntry entry = new LogEntry(message, type, logToConsole);
                _entriesLock.WaitOne();
                _entries.Add(entry);
                _entriesLock.ReleaseMutex();
            }
        }

        static string LogFileName()
        {
            string date = DateTime.Now.ToShortDateString();
            date = date.Replace('/', '-');
            string filename = "./Logs/" + date + ".txt";
            return filename;
        }

        static void LogThreadFunc()
        {
            while (true)
            {
                try
                {
                    _entriesLock.WaitOne();
                    LogEntry[] entries = _entries.ToArray();
                    _entries.Clear();
                    _entriesLock.ReleaseMutex();

                    if (entries.Length > 0)
                    {
                        FileStream fs = File.Open(LogFileName(), FileMode.Append);
                        StreamWriter sw = new StreamWriter(fs);

                        foreach (LogEntry e in entries)
                        {
                            string entryLine = e.GetText();
                            sw.WriteLine(entryLine);

                            if (e.LogToConsole || _alwaysPrintToConsole)
                                Console.WriteLine(entryLine);
                        }

                        sw.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw ex;
                }

                Thread.Sleep(100);
            }
        }

        public static bool AlwaysPrintToConsole
        {
            get { return _alwaysPrintToConsole; }
            set { _alwaysPrintToConsole = value; }
        }
    }
}
