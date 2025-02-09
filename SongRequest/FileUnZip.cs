using IPA.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using SongCore;
using ChzzkChat.Configuration;
using ChzzkChat.UI;

namespace ChzzkChat.SongRequest
{
    class FileUnZip
    {
        string CustomLevelPath = Path.Combine(UnityGame.InstallPath, "Beat Saber_Data", "CustomLevels");
        string FilePath = "";
        Loader Loader = new Loader();
        GetSongData getSongData = new GetSongData();

        public FileUnZip(string songCode)
        {
            Run(songCode);

            PluginConfig.Instance.Changed();
        }

        private void Run(string songCode)
        {
            string[] files = Directory.GetFiles(CustomLevelPath, String.Format($"{songCode} *.zip"));

            if (files.Length > 0)
            {
                FilePath = files[0];
                UnZipFile();

                Loader.RefreshSongs();
            }
            else
            {
                Plugin.Log.Error("Can't find zip file");
            }
        }

        private void UnZipFile()
        {
            try
            {
                string unZipPath = String.Format($"{FilePath.Substring(0, FilePath.LastIndexOf(".zip"))}");
                ZipFile.ExtractToDirectory(FilePath, unZipPath);
                File.Delete(FilePath);

                getSongData.GetFileInfoFromFile(unZipPath);
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.Message);
            }
        }
    }
}
