using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO.Compression;

namespace FaceDetectionUpdater
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            WriteToConsole("Starting updating process...");

            WriteToConsole("Given args:");
            foreach (var arg in args)
            {
                WriteToConsole(arg);
            }

            if (args.Length < 2)
            {
                WriteToConsole("Error! Missing parameters. Parameter count must be 2!");
                HoldConsole();
                return;
            }

            StartUpdate(args[0], args[1]);
        }

        private static void WriteToConsole(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }

        private static void HoldConsole()
        {
#if DEBUG
            Console.ReadKey();
#endif
        }

        private static async void StartUpdate(string applicationPath, string zipPath)
        {
            if(applicationPath == null || zipPath == null)
                return;

            try
            {
                Mutex mutex;

                while (Mutex.TryOpenExisting("FaceDetectionApp", out mutex))
                {
                    await Task.Delay(25);
                }
            }
            catch (Exception ex)
            {
                WriteToConsole("Error encountered while waiting for application close: " + ex);
                HoldConsole();
                return;
            }

            try
            {
                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    double i = 1;
                    foreach (var entry in archive.Entries)
                    {
                        WriteToConsole($"Processing... {Math.Round(i / archive.Entries.Count * 100)}%{Environment.NewLine}{entry.FullName}");

                        entry.ExtractToFile(Path.Combine(Path.GetDirectoryName(applicationPath), entry.FullName), true);

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToConsole("Couldn't extract zip: " + ex);
                HoldConsole();
            }

            try
            {
                Process.Start(applicationPath);
            }
            catch (Exception ex)
            {
                WriteToConsole("Couldn't start app: " + ex);
                throw;
            }

            HoldConsole();
            
        }
    }
}
