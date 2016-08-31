using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Linq;

namespace LeagueSandbox.GameServer.Core.Logic
{
    public static class Logger
    {
        public static void CurrentDomain_FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            if (e.Exception is InvalidCastException || e.Exception is System.Collections.Generic.KeyNotFoundException)
                return;
            WriteToLog.Log("A first chance exception was thrown", "EXCEPTION", ConsoleColor.Red);
            WriteToLog.Log(e.Exception.Message, "EXCEPTION", ConsoleColor.Red);
            WriteToLog.Log(e.ToString(), "EXCEPTION", ConsoleColor.Red);
        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs x)
        {
            WriteToLog.Log("An unhandled exception was thrown", "UNHANDLEDEXCEPTION");
            var ex = (Exception)x.ExceptionObject;
            WriteToLog.Log(ex.Message, "UNHANDLEDEXCEPTION", ConsoleColor.Red);
            WriteToLog.Log(ex.ToString(), "UNHANDLEDEXCEPTION", ConsoleColor.Red);
        }

        public static void Log(string line, string type = "LOG", ConsoleColor color = ConsoleColor.White)
        {
            WriteToLog.Log(line, type, color);
        }

        public static void LogCoreInfo(string line)
        {
            Log(line, "CORE_INFO", ConsoleColor.Cyan);
        }

        public static void LogCoreInfo(string format, params object[] args)
        {
            LogCoreInfo(string.Format(format, args));
        }

        public static void LogCoreWarning(string line)
        {
            Log(line, "CORE_WARNING", ConsoleColor.Yellow);
        }

        public static void LogCoreWarning(string format, params object[] args)
        {
            LogCoreWarning(string.Format(format, args));
        }

        public static void LogCoreError(string line)
        {
            Log(line, "CORE_ERROR", ConsoleColor.Red);
        }

        public static void LogCoreError(string format, params object[] args)
        {
            LogCoreError(string.Format(format, args));
        }
        public static void LogFullColor(string lines, string type = "LOG", ConsoleColor color = ConsoleColor.White)
        {
            WriteToLog.LogFullColor(lines, type, color);
        }
    }

    public static class WriteToLog
    {
        public static string ExecutingDirectory;
        public static string LogfileName;
        private static object locker = new object();

        public static void Log(string lines, string type = "LOG", ConsoleColor color = ConsoleColor.White)
        {
            var text = "";
            Game game = Game.Games.FirstOrDefault();
            if (game != null && game.GetMap() != null)
                text = game.GetMap().getGameTimeString();
            else
                text = "00:00:00";

            text += " ";

            lock (locker)
            {
                File.AppendAllText(Path.Combine(ExecutingDirectory, "Logs", LogfileName), text + Environment.NewLine);
                Console.ForegroundColor = color;
                Console.Write(text);
                Console.ResetColor();
                Console.Write(lines + Environment.NewLine);
            }
        }

        public static void LogFullColor(string lines, string type = "LOG", ConsoleColor color = ConsoleColor.White)
        {
            var text = "";
            Game game = Game.Games.FirstOrDefault();
            if (game != null && game.GetMap() != null)
                text = game.GetMap().getGameTimeString();
            else
                text = "00:00:00";

            text += " ";

            lock (locker)
            {
                File.AppendAllText(Path.Combine(ExecutingDirectory, "Logs", LogfileName), text + Environment.NewLine);
                Console.ForegroundColor = color;
                Console.Write(text);
                Console.Write(lines + Environment.NewLine);
                Console.ResetColor();
            }
        }

        public static void CreateLogFile()
        {
            //Generate A Unique file to use as a log file
            if (!Directory.Exists(Path.Combine(ExecutingDirectory, "Logs")))
                Directory.CreateDirectory(Path.Combine(ExecutingDirectory, "Logs"));
            LogfileName = string.Format("{0}T{1}{2}", DateTime.Now.ToShortDateString().Replace("/", "_"),
                DateTime.Now.ToShortTimeString().Replace(" ", "").Replace(":", "-"), "_" + LogfileName);
            var file = File.Create(Path.Combine(ExecutingDirectory, "Logs", LogfileName));
            file.Close();
        }
    }
}
