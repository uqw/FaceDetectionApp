using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Deserializers;

namespace FaceDetection.Model.Updater
{
    /// <summary>
    /// Handler for auto updates of the application
    /// </summary>
    public class UpdateHandler
    {
        #region Locals
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
                return LocaVersion.CompareTo(RemoteVersion) > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't get latest release from repo. Error: " + ex);
            }

            return false;
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
        #endregion
    }
}
