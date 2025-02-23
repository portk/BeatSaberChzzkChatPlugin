using ChzzkChat.Configuration;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ChzzkChat.SongRequest
{
    class SongDownloader
    {
        private WebClient webClient = new WebClient();
        private readonly string _baseUrl = "https://api.beatsaver.com/download/key/";
        private Uri downloadUri;
        public int progress { get; set; } = 0;
        Request request;

        public SongDownloader(int idx)
        {
            request = PluginConfig.Instance.RequestList[idx];
            downloadUri = new Uri(String.Format($"{_baseUrl}{request.SongCode}"));

            SetWebClientEvents();
            FileDownload();
        }

        private void SetWebClientEvents()
        {
            webClient.DownloadProgressChanged += (s, e) =>
            {
                // it fill request background color green;
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
                webClient.DownloadFileAsync(downloadUri, String.Format($@"{CustomLevelPathHelper.customLevelsDirectoryPath}/{request.SongName}.zip"));
            }
            catch (Exception ex)
            {
                
                Plugin.Log.Error(ex.Message + " Download Error");
            }
        }
    }
}
