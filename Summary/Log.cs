using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summary
{

    public static class LogAgregator
    {
        static List<LogEventArgs> _events = new List<LogEventArgs> ();

        public static List<LogEventArgs> Events
        {
            get
            {
                return _events;
            }

            private set
            {
                _events = value;
            }
        }

        public static void Log(LogEventArgs args)
        {
            _events.Add (args);
        }

        public static void ToFile(string FileName)
        {
            using (StreamWriter file =
           new StreamWriter (FileName))
            {
                foreach (LogEventArgs log in Events)
                {
                    file.WriteLine (string.Format ("[{0}] {1}", log.EventTime, log.Message));
                }
            }
        }
    }
    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(string message, DateTime eventTime)
        {
            Message = message;
            EventTime = eventTime;
        }

        [JsonProperty]
        public string Message
        {
            get;
            private set;
        }

        [JsonProperty]
        public DateTime EventTime
        {
            get;
            private set;
        }
    }
}
