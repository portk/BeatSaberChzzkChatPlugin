using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using ChzzkChat.Configuration;
using ChzzkChat.SongRequest;
using ChzzkChat.Util;
using HMUI;
using System;
using System.Collections.Generic;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

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

        [UIValue("request-list")]
        public IList<CustomCellInfo> requestList
        {
            get => RequestListUtil.RequestListToCustomCellInfoList();
        }

        public Action<Request> clickedRequest;
        private int selectedIdx = -1;

        [UIAction("request-click")]
        private void RequestClick(TableView tableVeiw, int idx)
        {
            Request request = PluginConfig.Instance.RequestList[idx];
            clickedRequest?.Invoke(request);
            selectedIdx = idx;
        }

        [UIAction("on-click-accept-btn")]
        private void OnClickAcceptBtn()
        {
            if (selectedIdx > -1)
            {
                requestListControl.AcceptRequest(selectedIdx);
                selectedIdx = -1;
            }
        }

        [UIAction("on-click-decline-btn")]
        private void OnClickDeclineBtn()
        {
            if (selectedIdx > -1)
            {
                requestListControl.DeclineRequest(selectedIdx);
                selectedIdx = -1;
            }
        }
    }
}
