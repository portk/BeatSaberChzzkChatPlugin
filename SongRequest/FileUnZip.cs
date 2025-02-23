using IPA.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using ChzzkChat.Configuration;
using System.Threading.Tasks;

namespace ChzzkChat.SongRequest
{
    class FileUnZip
    {
        public FileUnZip(string songCode)
        {
            string[] files = Directory.GetFiles(CustomLevelPathHelper.customLevelsDirectoryPath, String.Format($"{songCode} *.zip"));

            if (files.Length > 0)
            {
                string FilePath = files[0];

                try
                {
                    string unZipPath = String.Format($"{FilePath.Substring(0, FilePath.LastIndexOf(".zip"))}");
                    ZipFile.ExtractToDirectory(FilePath, unZipPath);
                    File.Delete(FilePath);

                    SongCore.Loader.Instance?.RefreshSongs(false);
                }
                catch (Exception ex)
                {
                    Plugin.Log.Error(ex.Message);
                }
            }
            else
            {
                Plugin.Log.Error("Can't find zip file");
            }

            PluginConfig.Instance.Changed();
        }
    }
}
