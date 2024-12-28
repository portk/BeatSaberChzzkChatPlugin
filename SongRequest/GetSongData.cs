using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace ChzzkChat.SongRequest
{
    class GetSongData 
    {
        public static GetSongData Instance { get; set; }

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

        public JObject GetFileInfoFromFile(string fileDirectory)
        {
            JObject result = new JObject(
                new JProperty("SongName",""),
                new JProperty("SongSubName", ""),
                new JProperty("SongAuthorName", ""),
                new JProperty("LevelAuthorName", "")
                );

            using (StreamReader reader = File.OpenText($@"{fileDirectory}\info.dat"))
            {
                JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                result["SongName"] = (string)o["_songName"];
                result["SongSubName"] = (string)o["_songSubName"];
                result["SongAuthorName"] = (string)o["_songAuthorName"];
                result["LevelAuthorName"] = (string)o["_levelAuthorName"];
            }

            Plugin.Log.Info($"{result["SongName"]} {result["SongSubName"]}\n{result["SongAuthorName"]} [{result["LevelAuthorName"]}]");

            return result;
        }
    }
}
