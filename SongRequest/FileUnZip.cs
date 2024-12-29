using IPA.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using SongCore;

namespace ChzzkChat.SongRequest
{
    class FileUnZip
    {
        string CustomLevelPath = Path.Combine(UnityGame.InstallPath, "Beat Saber_Data", "CustomLevels");
        string FilePath = "";
        Loader Loader = new Loader();

        public FileUnZip(string songCode)
        {
            Run(songCode);
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

                GetSongData.Instance.GetFileInfoFromFile(unZipPath);
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.Message);
            }
        }
    }
}
