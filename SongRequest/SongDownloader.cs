using ChzzkChat.Configuration;
using IPA.Utilities;
using System;
using System.IO;
using System.Net;

namespace ChzzkChat.SongRequest
{
    class SongDownloader
    {
        private WebClient webClient = new WebClient();
        private readonly string _baseUrl = "https://api.beatsaver.com/download/key/";
        private Uri downloadUri;
        public int progress { get; set; } = 0;
        private readonly string CustomLevelPath = Path.Combine(UnityGame.InstallPath, "Beat Saber_Data", "CustomLevels");
        Request request;

        public SongDownloader(int idx)
        {
            request = PluginConfig.Instance.RequestList[idx];
            
            // Set Download Url
            downloadUri = new Uri(String.Format($"{_baseUrl}{request.SongCode}"));

            SetWebClientEvents();
            FileDownload();
        }

        private void SetWebClientEvents()
        {
            webClient.DownloadProgressChanged += (s, e) =>
            {
                progress = e.ProgressPercentage;
            };

            webClient.DownloadFileCompleted += (s, e) =>
            {
                new FileUnZip(request.SongCode);
            };
        }

        private void FileDownload()
        {
            try
            {
                webClient.DownloadFileAsync(downloadUri, String.Format($@"{CustomLevelPath}/{request.SongName}.zip"));
            }
            catch (Exception ex)
            {
                
                Plugin.Log.Error(ex.Message + " Download Error");
            }
        }
    }
}
