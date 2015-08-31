using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lemmator
{
    class Logger
    {
        private const int LOG_LEVEL = (int)LogLevel.Debug;

        private static ConsoleColor defaultConsoleColor = Console.ForegroundColor;

        public static void logError(string message, params object[] args)
        {
            internalLog(LogLevel.Error, message, args);
        }

        public static void logInfo(string message, params object[] args)
        {
            internalLog(LogLevel.Info, message, args);
        }

        private static void internalLog(LogLevel level, string message, params object[] args)
        {
            if ((int)level < LOG_LEVEL)
                return;

            Console.ForegroundColor = getConsoleColor(level);

            string messagePrefix = getMessagePrefix(level);

            Console.WriteLine(messagePrefix + message, args);

            resetConsoleColor();
        }

        private static ConsoleColor getConsoleColor(LogLevel level)
        {
            ConsoleColor result = ConsoleColor.White;

            switch (level)
            {
                case LogLevel.Error:
                    result = ConsoleColor.Red;
                    break;

                case LogLevel.Info:
                    result = ConsoleColor.Cyan;
                    break;
            }

            return result;
        }

        private static string getMessagePrefix(LogLevel level)
        {
            string result = "";

            switch (level)
            {
                case LogLevel.Error:
                    result = "[ERROR]";
                    break;

                case LogLevel.Info:
                    result = "[INFO]";
                    break;
            }

            result += " ";

            return result;
        }

        private static void resetConsoleColor()
        {
            Console.ForegroundColor = defaultConsoleColor;
        }
    }
}
