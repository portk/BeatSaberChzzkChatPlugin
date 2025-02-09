using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using ChzzkChat.Configuration;
using ChzzkChat.SongRequest;
using HMUI;
using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Components;

namespace ChzzkChat.UI
{
    internal class LeftPanelController : BSMLAutomaticViewController
    {
        private RequestListControl requestListControl = new RequestListControl();
        
        [UIValue("requestQueState")]
        public bool RequestQueState
        {
            get => PluginConfig.Instance.RequestQueOpen;
            set
            {
                PluginConfig.Instance.RequestQueOpen = value;
                NotifyPropertyChanged();
            }
        }

        #region RequestList
        [UIComponent("request-list")]
        public CustomCellListTableData customRequestList = null;
        public Action<Request> clickedRequest;

        private int selectedIdx = -1;
        bool reloadData = true;

        [UIValue("request-list-data")]
        public List<object> requestListData
        {
            get
            {
                List<object> list = new List<object>();

                foreach (var i in PluginConfig.Instance.RequestList)
                {
                    list.Add((object)i);
                }

                return list;
            }
        }

        [UIAction("request-click")]
        void RequestClick(TableView tableVeiw, Request request)
        {
            if (request.SongName == null)
            {
                tableVeiw.ReloadData();
            }

            selectedIdx = requestListData.FindIndex(song => song.Equals(request));

            Plugin.Log.Debug($"{selectedIdx}");
        }

        [UIAction("on-click-accept-btn")]
        private void OnClickAcceptBtn()
        {
            if (selectedIdx > -1)
            {
                requestListControl.AcceptRequest(selectedIdx);
            }

        }

        [UIAction("on-click-decline-btn")]
        private void OnClickDeclineBtn()
        {
            if (selectedIdx > -1)
            {
                requestListControl.DeclineRequest(selectedIdx);
                selectedIdx = -1;

                NotifyPropertyChanged();
                customRequestList.tableView.ReloadData();
            }
        }

        public void Update()
        {
            if (requestListData == null || customRequestList.tableView == null)
            {
                return;
            }

            if (reloadData)
            {
                customRequestList.data = requestListData;
                customRequestList.tableView.ReloadData();
            }

            if (selectedIdx > -1)
            {
                customRequestList.tableView.SelectCellWithIdx(selectedIdx);
            }
        }

        #endregion RequestList
    }
}
