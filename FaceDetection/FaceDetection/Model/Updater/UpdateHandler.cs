using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
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
        /// <summary>
        /// Occurs when the update download is completed.
        /// </summary>
        public event UpdateDownloadCompletedEventHandler UpdateDownloadCompleted;
        /// <summary>
        /// The delegate for the <see cref="UpdateHandler.UpdateDownloadCompleted"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        public delegate void UpdateDownloadCompletedEventHandler(object sender, UpdateDownloadCompletedArgs e);

        /// <summary>
        /// The event args for the <see cref="UpdateHandler.UpdateDownloadCompleted"/> event.
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class UpdateDownloadCompletedArgs : EventArgs
        {
            /// <summary>
            /// Gets a value indicating whether the operation was aborted.
            /// </summary>
            /// <value>
            ///   <c>true</c> if aborted; otherwise, <c>false</c>.
            /// </value>
            public bool Aborted { get; }
            /// <summary>
            /// Gets the exception.
            /// </summary>
            /// <value>
            /// The exception.
            /// </value>
            public Exception Exception { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateDownloadCompletedArgs"/> class.
            /// </summary>
            /// <param name="aborted">if set to <c>true</c> [aborted].</param>
            /// <param name="exception">The exception.</param>
            public UpdateDownloadCompletedArgs(bool aborted, Exception exception)
            {
                Aborted = aborted;
                Exception = exception;
            }
        }
        /// <summary>
        /// Called when the update download was completed.
        /// </summary>
        /// <param name="e">The args.</param>
        protected virtual void OnUpdateDownloadCompleted(UpdateDownloadCompletedArgs e)
        {
            UpdateDownloadCompleted?.Invoke(this, e);
        }
        #endregion

        #region UpdateDownloadProgressChanged        
        /// <summary>
        /// Occurs when the update download progress changed.
        /// </summary>
        public event UpdateDownloadProgressChangedEventhandler UpdateDownloadProgressChanged;

        /// <summary>
        /// The delegate for the <see cref="UpdateHandler.UpdateDownloadProgressChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        public delegate void UpdateDownloadProgressChangedEventhandler(object sender, UpdateDownloadProgressChangedArgs e);

        /// <summary>
        /// The event args fpr the <see cref="UpdateHandler.UpdateDownloadProgressChanged"/> event.
        /// </summary>
        /// <seealso cref="System.EventArgs" />
        public class UpdateDownloadProgressChangedArgs : EventArgs
        {
            /// <summary>
            /// Gets the megabytes to receive.
            /// </summary>
            /// <value>
            /// The megabytes to receive.
            /// </value>
            public double MegabytesToReceive { get; }

            /// <summary>
            /// Gets the received megabytes.
            /// </summary>
            /// <value>
            /// The received megabytes.
            /// </value>
            public double MegabytesReceived { get; }

            /// <summary>
            /// Gets the progress percentage.
            /// </summary>
            /// <value>
            /// The progress percentage.
            /// </value>
            public int ProgressPercentage { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="UpdateDownloadProgressChangedArgs"/> class.
            /// </summary>
            /// <param name="bytesToReceive">The bytes to receive.</param>
            /// <param name="bytesReceived">The bytes received.</param>
            /// <param name="progressPercentage">The progress percentage.</param>
            public UpdateDownloadProgressChangedArgs(long bytesToReceive, long bytesReceived, int progressPercentage)
            {
                MegabytesToReceive = Math.Round(bytesToReceive/1000000.0, 2, MidpointRounding.ToEven);
                MegabytesReceived = Math.Round(bytesReceived/1000000.0, 2, MidpointRounding.ToEven);
                ProgressPercentage = progressPercentage;
            }
        }

        /// <summary>
        /// Called when the update download progress changed.
        /// </summary>
        /// <param name="e">The args.</param>
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
        /// <summary>
        /// Gets the local version.
        /// </summary>
        /// <value>
        /// The local version.
        /// </value>
        public Version LocalVersion { get; }

        /// <summary>
        /// Gets the remote version.
        /// </summary>
        /// <value>
        /// The remote version.
        /// </value>
        public Version RemoteVersion { get; private set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateHandler"/> class.
        /// </summary>
        public UpdateHandler()
        {
            LocalVersion = GetAssemblyVersion();
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
                return LocalVersion.CompareTo(RemoteVersion) < 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't get latest release from repo. Error: " + ex);
            }

            return false;
        }

        /// <summary>
        /// Cancels the download.
        /// </summary>
        public void CancelDownload()
        {
            _downloader?.CancelAsync();
        }

        /// <summary>
        /// Downloads the update.
        /// </summary>
        /// <returns></returns>
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