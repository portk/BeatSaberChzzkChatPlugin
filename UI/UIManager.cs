using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;
using UnityEngine;

namespace ChzzkChat.UI
{
    internal class UIManager
    {
        public static UIManager Instance = new UIManager();

        private SettingsController SettingsMenuInstance { get; set; }
        public LeftPanelController LeftPanelControllerInstance { get; set; }

        public UIManager()
        {
            SettingsMenuInstance = new SettingsController();
            LeftPanelControllerInstance = new LeftPanelController();
        }

        public void AddSettingsMenu() => BSMLSettings.instance.AddSettingsMenu(nameof(ChzzkChat), "ChzzkChat.UI.settings.bsml", SettingsMenuInstance);
        public void AddLeftPanel() => GameplaySetup.instance.AddTab(nameof(ChzzkChat), "ChzzkChat.UI.leftPanel.bsml", LeftPanelControllerInstance);

        public void RemoveSettingsMenu() => BSMLSettings.instance.RemoveSettingsMenu(SettingsMenuInstance);
        public void RemoveLeftPanel() => GameplaySetup.instance.RemoveTab(nameof(ChzzkChat));
    }
    
}
