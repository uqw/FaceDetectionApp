using System;
using System.Diagnostics;
using System.IO;

namespace FaceDetection.Model
{
    /// <summary>
    /// Public Logger class. Logs the input to the diagnostics log created in %localappdata%
    /// </summary>
    public static class Logger
    {
        private static readonly string LogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, "logs");
        private static readonly string CurrentLog = Path.Combine(LogPath, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".log");

        /// <summary>
        /// Logs the given string to the diagnostics log
        /// </summary>
        /// <param name="content">The message that should be logged</param>
        private static void Log(string content)
        {
            try
            {
                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);

                // Adds a prefix at the beggining and a new line at the end to the message that should be logged
                // Output would be: 
                // [27.03.2016 20:16:43:234]: Example message
                var text = "[" + DateTime.Today.ToString("dd.MM.yyyy ") +  DateTime.Now.ToString("HH:mm:ss:fff") + "]: " + content + Environment.NewLine;
                File.AppendAllText(CurrentLog, text);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// Logs information data
        /// </summary>
        /// <param name="content"></param>
        public static void Info(string content)
        {
            Log("[Info] " + content);
        }

        /// <summary>
        /// Logs warnings
        /// </summary>
        /// <param name="content"></param>
        public static void Warning(string content)
        {
            Log("[Warning] " + content);
            Debug.WriteLine("Logged warning: " + content);
        }

        /// <summary>
        /// Logs errors
        /// </summary>
        /// <param name="content"></param>
        public static void Error(string content)
        {
            Log("[Error] " + content);
            Debug.WriteLine("Logged error: " + content);
        }
    }
}
