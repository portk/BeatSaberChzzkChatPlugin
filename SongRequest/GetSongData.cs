using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace ChzzkChat.SongRequest
{
    class GetSongData 
    {
        public string GetFileDataFromWeb(string songCode)
        {
            WebClient webClient = new WebClient();
            Uri downloadUri = new Uri(String.Format($"https://api.beatsaver.com/download/key/{songCode}"));
            string fileName = "";

            try
            {
                var data = webClient.DownloadData(downloadUri);
                string header = webClient.ResponseHeaders["Content-Disposition"] ?? string.Empty;
                const string filename = "filename=";
                int index = header.LastIndexOf(filename, StringComparison.OrdinalIgnoreCase);
                if (index > -1)
                {
                    fileName = header.Substring(index + filename.Length);
                    fileName = fileName.Substring(1, fileName.Length - 2);
                }

                return fileName;
            }
            catch
            {
                return fileName;
            }
        }
    }
}
