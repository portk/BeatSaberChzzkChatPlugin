using ChzzkChat.Configuration;
using IPA.Utilities;
using System;
using System.IO;
using System.Threading;

namespace ChzzkChat.SongRequest
{
    class RequestListControl
    {
        bool isListFull = false;

        public RequestListControl()
        {
            ThreadPool.SetMaxThreads(2, 2);
        }

        public bool CheckList(string songCode)
        {
            foreach (Request song in PluginConfig.Instance.RequestList)
            {
                if (song.SongCode == songCode) return true;
            }

            return false;
        }

        public void GetRequest(string songCode)
        {
            if (CheckList(songCode))
            {
                Plugin.Log.Info(String.Format($"{songCode} Already Requested."));

            }
            else if (isListFull)
            {
                Plugin.Log.Info(String.Format("List is Full."));
            }
            else
            {
                if (PluginConfig.Instance.RequestList.Count >= PluginConfig.Instance.RequestMaxCount) isListFull = true;

                Request request = new Request(songCode, "Searching information ...");

                PluginConfig.Instance.RequestList.Add(request);

                Plugin.Log.Info(request.ToString());

                ThreadPool.QueueUserWorkItem(CheckInfo, songCode);
            }
        }

        public void CheckInfo(object objSongCode)
        {
            string songCode = (string)objSongCode;
            string songName = GetSongData.Instance.GetFileDataFromWeb(songCode);

            int idx = 0;
            foreach (Request item in PluginConfig.Instance.RequestList)
            {
                if (item.SongCode == songCode)
                {
                    break;
                }
                idx += 1;
            }

            if (songName != "")
            {
                songName = songName.Substring(0, songName.IndexOf(".zip"));
                Request temp = PluginConfig.Instance.RequestList[idx];
                temp.SongName = songName;
                PluginConfig.Instance.RequestList[idx] = temp;
            }
            else
            {
                Plugin.Log.Info($"{songCode} is Call Off due to Song information Not Found.");
                DeclineRequest(idx);
            }
        }

        public void AcceptRequest(int idx)
        {
            string CustomLevel = Path.Combine(UnityGame.InstallPath, "Beat Saber_Data", "CustomLevels");

            string[] dirArray = Directory.GetDirectories(CustomLevel, String.Format($"{PluginConfig.Instance.RequestList[idx].SongCode} *"));
            if (dirArray.Length > 0)
            {
                GetSongData.Instance.GetFileInfoFromFile(dirArray[0]);
            }
            else
            {
                string[] files = Directory.GetFiles(CustomLevel, String.Format($"{PluginConfig.Instance.RequestList[idx].SongCode} *.zip"));
                if (files.Length > 0)
                {
                    Plugin.Log.Debug("File UnZip");

                    new FileUnZip(PluginConfig.Instance.RequestList[idx].SongCode);
                }
                else
                {
                    Plugin.Log.Debug("File Download");

                    new SongDownloader(idx);
                }
            }
        }

        public void DeclineRequest(int idx)
        {
            PluginConfig.Instance.RequestList.RemoveAt(idx);

            if (PluginConfig.Instance.RequestList.Count <= PluginConfig.Instance.RequestMaxCount) isListFull = false;
        }
    }
}
