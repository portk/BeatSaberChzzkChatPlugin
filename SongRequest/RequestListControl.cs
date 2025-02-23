using ChzzkChat.Configuration;
using IPA.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ChzzkChat.SongRequest
{
    class RequestListControl
    {
        private bool isListFull = false;

        private GetSongData getSongData = new GetSongData();

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
                Plugin.Log.Debug(String.Format($"{songCode} Already Requested."));
            }
            else if (isListFull)
            {
                Plugin.Log.Debug(String.Format("List is Full."));
            }
            else
            {
                if (PluginConfig.Instance.RequestList.Count >= PluginConfig.Instance.RequestMaxCount) isListFull = true;

                Request request = new Request(songCode, "Searching information ...");

                PluginConfig.Instance.RequestList.Add(request);

                Plugin.Log.Debug(request.ToString());

                ThreadPool.QueueUserWorkItem(CheckInfo, songCode);

                PluginConfig.Instance.Changed();
            }
        }

        public void CheckInfo(object objSongCode)
        {
            string songCode = (string)objSongCode;
            string songName = getSongData.GetFileDataFromWeb(songCode);

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

                PluginConfig.Instance.Changed();
            }
            else
            {
                Plugin.Log.Debug($"{songCode} is Call Off due to Song information Not Found.");
                DeclineRequest(idx);
            }

            PluginConfig.Instance.Changed();
        }

        public void AcceptRequest(int idx)
        {
            string[] dirArray = Directory.GetDirectories(CustomLevelPathHelper.customLevelsDirectoryPath, String.Format($"{PluginConfig.Instance.RequestList[idx].SongCode} *"));
            
            if (dirArray.Length > 0)
            {
                PluginConfig.Instance.Changed();
            }
            else
            {
                FileReady(idx);
            }

            // it change request background color to red;
            LevelSelect(PluginConfig.Instance.RequestList[idx].SongName);
        }

        public void DeclineRequest(int idx)
        {
            PluginConfig.Instance.RequestList.RemoveAt(idx);

            if (PluginConfig.Instance.RequestList.Count <= PluginConfig.Instance.RequestMaxCount) isListFull = false;

            PluginConfig.Instance.Changed();
        }

        async public Task LevelSelect(string songName)
        {
            int songIndex = 0;
            string songHash = "", cacheData = "";

            LevelCollectionViewController levelCollectionViewController = Resources.FindObjectsOfTypeAll<LevelCollectionViewController>().FirstOrDefault();

            var levelsTableView = levelCollectionViewController.GetField<LevelCollectionTableView, LevelCollectionViewController>("_levelCollectionTableView");
            List<IPreviewBeatmapLevel> beatmaps = levelsTableView.GetField<IReadOnlyList<IPreviewBeatmapLevel>, LevelCollectionTableView>("_previewBeatmapLevels").ToList();

            try
            {
                cacheData = File.ReadAllText(SongCore.Utilities.Hashing.cachedHashDataPath,Encoding.UTF8);
            }
            catch
            {
                Plugin.Log.Error("Can't Read CacheFile");
                return;
            }

            JObject songData = JObject.Parse(cacheData);

            songHash = (string)songData[$".\\Beat Saber_Data\\CustomLevels\\{songName}"]["songHash"];
            songIndex = beatmaps.FindIndex(x => (x.levelID.StartsWith("custom_level_" + songHash)));

            levelCollectionViewController?.SelectLevel(beatmaps[songIndex]);
        }

        public void FileReady(object input)
        {
            int idx = (int)input;
            string songCode = PluginConfig.Instance.RequestList[idx].SongCode;

            string[] files = Directory.GetFiles(CustomLevelPathHelper.customLevelsDirectoryPath, String.Format($"{songCode} *.zip"));
            if (files.Length > 0)
            {
                new FileUnZip(songCode);
            }
            else
            {
                new SongDownloader(idx);
            }
        }
    }
}
