using StardewModdingAPI;
using System;

namespace BugNet
{
    class Log
    {
        public static void trace(String str)
        {
            BugNetMod.instance.Monitor.Log(str, LogLevel.Trace);
        }

        public static void debug(String str)
        {
            BugNetMod.instance.Monitor.Log(str, LogLevel.Debug);
        }

        public static void info(String str)
        {
            BugNetMod.instance.Monitor.Log(str, LogLevel.Info);
        }

        public static void warn(String str)
        {
            BugNetMod.instance.Monitor.Log(str, LogLevel.Warn);
        }

        public static void error(String str)
        {
            BugNetMod.instance.Monitor.Log(str, LogLevel.Error);
        }
    }
}
