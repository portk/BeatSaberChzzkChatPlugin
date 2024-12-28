using IPA.Utilities;
using System;
using System.IO;
using System.IO.Compression;

namespace ChzzkChat.SongRequest
{
    class FileUnZip
    {
        string CustomLevelPath = Path.Combine(UnityGame.InstallPath, "Beat Saber_Data", "CustomLevels");
        string filePath = "";

        public FileUnZip(string songCode)
        {
            Run(songCode);
        }

        private void Run(string songCode)
        {
            string[] files = Directory.GetFiles(CustomLevelPath, String.Format($"{songCode} *.zip"));

            if (files.Length > 0)
            {
                filePath = files[0];
                UnZipFile();

                // 파일 리로드 시켜야함
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
                string unZipPath = String.Format($"{filePath.Substring(0, filePath.LastIndexOf(".zip"))}");
                ZipFile.ExtractToDirectory(filePath, unZipPath);
                File.Delete(filePath);

                GetSongData.Instance.GetFileInfoFromFile(unZipPath);
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex.Message);
            }
        }
    }
}
