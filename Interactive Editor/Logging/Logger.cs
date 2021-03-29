using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Logging
{
    public static class InspectorLogger
    {
        public static bool EnableVerbose = false;
        public static bool EnableDebug = false;
        public static bool EnableLog = true;
        private static DateTime _Time;
        private static DateTime Time
        {
            get
            {
                if (_Time == null)
                {
                    _Time = new DateTime();
                }
                return _Time;
            }
        }

        public static void Log(string content)
        {

            var t = DateTime.UtcNow.Subtract(Time);

            // $"[day:hour:min:sec:ms]"
            var timeString =
                "[" +
                (t.Hours > 0 ? $"{t.Hours       }:" : $"") +
                (t.Minutes > 0 ? $"{t.Minutes     }:" : $"") +
                (t.Seconds > 0 ? $"{t.Seconds     }:" : $"") +
                (t.Milliseconds > 0 ? $"{t.Milliseconds}" : $"") +
                "]";

            var formatedMessage = $"{timeString} {content}";
            if (!formatedMessage.EndsWith(";"))
            {
                formatedMessage += ";";
            }
            Console.WriteLine(formatedMessage);
        }
    }
    public static class Logger
    {
        public static void Log(string content)
        {
            if (InspectorLogger.EnableLog)
                InspectorLogger.Log(content);
        }
    }
    public static class Debug
    {
        public static void Log(string content)
        {
            if (InspectorLogger.EnableDebug)
                InspectorLogger.Log(content);
        }
    }
    public static class Verbose
    {
        public static void Log(string content)
        {
            if (InspectorLogger.EnableVerbose)
                InspectorLogger.Log(content);
        }
    }

}
