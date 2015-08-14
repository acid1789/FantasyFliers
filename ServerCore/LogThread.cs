using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using NetworkCore;

namespace ServerCore
{
    public class LogThread : LogInterface
    {
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
        
        public LogThread() : base()
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

        public void Shutdown()
        {
            if (_logThread != null)
            {
                _logThread.Abort();
                _logThread = null;

                _entriesLock.WaitOne();
                _entries.Clear();
                _entries = null;
                _entriesLock.ReleaseMutex();
            }
        }

        public override void Log(LogMessageType type, bool logToConsole, string message)
        {
            if (_entries != null)
            {
                LogEntry entry = new LogEntry(message, type, logToConsole);
                _entriesLock.WaitOne();
                _entries.Add(entry);
                _entriesLock.ReleaseMutex();
            }
        }

        public static LogThread GetLog() { return (LogThread)_log; }    

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
        
    }
}
