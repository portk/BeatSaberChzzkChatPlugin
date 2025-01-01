using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using ChzzkChat.Configuration;
using ChzzkChat.Util;
using HMUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BeatSaberMarkupLanguage.Components.CustomListTableData;

namespace ChzzkChat.UI
{
    internal class LeftPanelController : BSMLAutomaticViewController
    {
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

        [UIAction("request-click")]
        private void requestClick(TableView tableVeiw, int idx)
        {
            Request request = PluginConfig.Instance.RequestList[idx];
            clickedRequest?.Invoke(request);
        }
    }
}
