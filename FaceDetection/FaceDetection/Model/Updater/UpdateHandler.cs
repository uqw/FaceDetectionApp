using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using FaceDetection.Model.Recognition;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Deserializers;
using DataFormat = RestSharp.DataFormat;

namespace FaceDetection.Model.Updater
{
    /// <summary>
    /// Handler for auto updates of the application
    /// </summary>
    public class UpdateHandler
    {
        #region Events
        #region UpdateDownloadCompleted
        public event UpdateDownloadCompletedEventHandler UpdateDownloadCompleted;
        public delegate void UpdateDownloadCompletedEventHandler(object sender, UpdateDownloadCompletedArgs e);
        public class UpdateDownloadCompletedArgs : EventArgs
        {
            public bool Aborted { get; }
            public Exception Exception { get; }

            public UpdateDownloadCompletedArgs(bool aborted, Exception exception)
            {
                Aborted = aborted;
                Exception = exception;
            }
        }

        protected virtual void OnUpdateDownloadCompleted(UpdateDownloadCompletedArgs e)
        {
            UpdateDownloadCompleted?.Invoke(this, e);
        }
        #endregion

        #region UpdateDownloadProgressChanged
        public event UpdateDownloadProgressChangedEventhandler UpdateDownloadProgressChanged;
        public delegate void UpdateDownloadProgressChangedEventhandler(object sender, UpdateDownloadProgressChangedArgs e);
        public class UpdateDownloadProgressChangedArgs : EventArgs
        {
            public double MegabytesToReceive { get; }
            public double MegabytesReceived { get; }
            public int ProgressPercentage { get; }

            public UpdateDownloadProgressChangedArgs(long bytesToReceive, long bytesReceived, int progressPercentage)
            {
                MegabytesToReceive = Math.Round(bytesToReceive/1000000.0, 2, MidpointRounding.ToEven);
                MegabytesReceived = Math.Round(bytesReceived/1000000.0, 2, MidpointRounding.ToEven);
                ProgressPercentage = progressPercentage;
            }
        }

        protected virtual void OnUpdateDownloadProgressChanged(UpdateDownloadProgressChangedArgs e)
        {
            UpdateDownloadProgressChanged?.Invoke(this, e);
        }
        #endregion
        #endregion

        #region Locals
        private string _downloadUrl;
        private WebClient _downloader;
        private string _updatePackagePath;
        #endregion

        #region Properties
        public Version LocaVersion { get; }
        public Version RemoteVersion { get; private set; }
        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateHandler"/> class.
        /// </summary>
        public UpdateHandler()
        {
            LocaVersion = GetAssemblyVersion();
        }

        #region Methods        
        /// <summary>
        /// Determines whether an update is available.
        /// </summary>
        /// <returns>True if an update is available otherwise false.</returns>
        public async Task<bool> IsUpdateAvailable()
        {
            try
            {
                var client = new RestClient(Properties.Settings.Default.APIUrl);
                var request =
                    new RestRequest(
                        $"/repos/{Properties.Settings.Default.RepoOwner}/{Properties.Settings.Default.RepoName}/releases/latest",
                        Method.GET)
                    {
                        RequestFormat = DataFormat.Json,
                    };
                var result = await client.ExecuteGetTaskAsync(request);

                if (result.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine("Couldn't get latest release from repo. Result code was " + result.StatusCode);
                    return false;
                }

                var json = JObject.Parse(result.Content);
                RemoteVersion = ParseVersion(json["tag_name"].ToString());
                foreach (var child in json["assets"].Children())
                {
                    if (child["name"].ToString().Contains(Properties.Settings.Default.AssetName))
                    {
                        _downloadUrl = child["browser_download_url"].ToString();
                        break;
                    }
                }
                return LocaVersion.CompareTo(RemoteVersion) < 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't get latest release from repo. Error: " + ex);
            }

            return false;
        }

        public void CancelDownload()
        {
            _downloader?.CancelAsync();
        }

        public async Task DownloadUpdate()
        {
            try
            {
                using (_downloader = new WebClient())
                {
                    _downloader.DownloadDataCompleted += (sender, args) =>
                    {
                        try
                        {
                            _updatePackagePath = Path.GetTempFileName();
                            File.WriteAllBytes(_updatePackagePath, args.Result);

                            OnUpdateDownloadCompleted(new UpdateDownloadCompletedArgs(args.Cancelled, args.Error));

                            StartUpdate();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Error saving download result: " + ex);
                        }
                    };

                    _downloader.DownloadProgressChanged += (sender, args) =>
                    {
                        OnUpdateDownloadProgressChanged(new UpdateDownloadProgressChangedArgs(args.TotalBytesToReceive, args.BytesReceived, args.ProgressPercentage));
                    };

                    await _downloader.DownloadDataTaskAsync(new Uri(_downloadUrl));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to download update: " + ex);
                OnUpdateDownloadCompleted(new UpdateDownloadCompletedArgs(true, ex));
            }
            
        }

        private Version ParseVersion(string versionText)
        {
            var match = Regex.Match(versionText, @"(\d+)\.(\d+)\.(\d+)(\.\d)?", RegexOptions.None);

            if (match.Success)
            {
                try
                {
                    var first = int.Parse(match.Groups[1].Value);
                    var second = int.Parse(match.Groups[2].Value);
                    var third = int.Parse(match.Groups[3].Value);

                    var fourth = 0;

                    if (match.Groups[4].Value != "")
                    {
                        fourth = int.Parse(match.Groups[4].Value);
                    }

                    return new Version(first, second, third, fourth);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in parsing version. Given version: {versionText}. Error: {ex}");
                }
            }

            return new Version();
        }

        private Version GetAssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        private void StartUpdate()
        {
            if(string.IsNullOrEmpty(_updatePackagePath))
                return;

            var updaterName = typeof(FaceDetectionUpdater.Program).Assembly.GetName().Name + ".exe";
            var tempPath = Path.Combine(Path.GetTempPath(), updaterName);

            try
            {
                File.Copy(updaterName, tempPath, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not copy updater: " + ex);
                return;
            }

            try
            {
                Process.Start(tempPath, $"{Assembly.GetEntryAssembly().Location} {_updatePackagePath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not start updater " + ex);
                return;
            }

            Environment.Exit(0);
        }
        #endregion
    }
}
