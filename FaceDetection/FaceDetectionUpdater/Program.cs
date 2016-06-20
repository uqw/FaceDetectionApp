using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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

            Thread.Sleep(1000);

            try
            {
                Mutex mutex;

                while (Mutex.TryOpenExisting("FaceDetectionApp", out mutex))
                {
                    Thread.Sleep(25);
                }
            }
            catch (Exception ex)
            {
                WriteToConsole("Error encountered while waiting for application close: " + ex);
                HoldConsole();
                return;
            }

            StartUpdate(args[0], args[1]);

            HoldConsole();
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

        private static void StartUpdate(string applicationPath, string zipPath)
        {
            if(applicationPath == null || zipPath == null)
                return;

            var applicationDir = Path.GetDirectoryName(applicationPath);

            var di = new DirectoryInfo(applicationDir);

            foreach (var file in di.GetFiles())
            {
                try
                {
                    WriteToConsole("Deleting " + file);
                    file.Delete();
                }
                catch (Exception)
                {
                    WriteToConsole("Failed to delete " + file);
                }
            }

            foreach (var dir in di.GetDirectories())
            {
                try
                {
                    WriteToConsole("Deleting " + dir);
                    dir.Delete(true);
                }
                catch (Exception ex)
                {
                    WriteToConsole("Failed to delete " + ex);
                }
            }

            try
            {
                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    double i = 1;
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.Name.ToLower().EndsWith("xml") && !entry.Name.ToLower().EndsWith("pdb"))
                        {
                            WriteToConsole($"Processing... {Math.Round(i / archive.Entries.Count * 100)}%{Environment.NewLine}{entry.FullName}");

                            var directory = Path.GetDirectoryName(entry.FullName);
                            if(!string.IsNullOrWhiteSpace(directory))
                                Directory.CreateDirectory(directory);

                            entry.ExtractToFile(Path.Combine(Path.GetDirectoryName(applicationPath), entry.FullName), true);
                        }
                        else
                        {
                            WriteToConsole("Skipping file " + entry.FullName);
                        }

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToConsole("Couldn't extract zip: " + ex);
                return;
            }

            try
            {
                Process.Start(applicationPath);
            }
            catch (Exception ex)
            {
                WriteToConsole("Couldn't start app: " + ex);
            }
        }
    }
}
