using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Misc
{
    public class LogEventArgs
    {
        public string Value;
        public int Severity;
    }

    public static class Logger
    {
        public delegate void LogHandler(object sender, LogEventArgs e);
        private static event LogHandler OnLog;
        public static void AtatchLogHandler(LogHandler e) => OnLog += e;
        public static void DoLog(object sender, string value, int severity = 0)
        {
            OnLog?.Invoke(sender, new LogEventArgs() { Value = value, Severity = severity });
            Console.WriteLine(value);
        }
    }
}
