using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using ChzzkChat.Configuration;

namespace ChzzkChat.UI
{
    internal class SettingsController : NotifiableBase
    {
        [UIValue("requestMaxCnt")]
        public int requestMaxCnt
        {
            get => PluginConfig.Instance.RequestMaxCount;
            set {
                PluginConfig.Instance.RequestMaxCount = value;
                NotifyPropertyChanged();
                }
        }
        [UIAction("onClickRequestMaxCntUp")]
        private void OnClickRequestMaxCntUp()
        {
            requestMaxCnt += 1;
        }
        [UIAction("onClickRequestMaxCntDown")]
        private void OnClickRequestMaxCntDown()
        {
            if (requestMaxCnt > 1)
            {
                requestMaxCnt -= 1;
            }
        }
    }
}
