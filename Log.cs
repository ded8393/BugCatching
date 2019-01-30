using StardewModdingAPI;
using System;

namespace BugCatching
{
    class Log
    {
        public static void trace(String str)
        {
            BugCatchingMod.instance.Monitor.Log(str, LogLevel.Trace);
        }

        public static void debug(String str)
        {
            BugCatchingMod.instance.Monitor.Log(str, LogLevel.Debug);
        }

        public static void info(String str)
        {
            BugCatchingMod.instance.Monitor.Log(str, LogLevel.Info);
        }

        public static void warn(String str)
        {
            BugCatchingMod.instance.Monitor.Log(str, LogLevel.Warn);
        }

        public static void error(String str)
        {
            BugCatchingMod.instance.Monitor.Log(str, LogLevel.Error);
        }
    }
}
