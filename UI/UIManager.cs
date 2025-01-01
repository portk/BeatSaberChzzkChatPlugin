using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Settings;

namespace ChzzkChat.UI
{
    internal class UIManager
    {
        private static SettingsController SettingsMenuInstance { get; set; }
        private static LeftPanelController LeftPanelControllerInstance { get; set; }

        public static void AddSettingsMenu() => BSMLSettings.instance.AddSettingsMenu(nameof(ChzzkChat), "ChzzkChat.UI.settings.bsml", new SettingsController());
        public static void RemoveSettingsMenu() => BSMLSettings.instance.RemoveSettingsMenu(SettingsMenuInstance);

        public static void AddLeftPanel() => GameplaySetup.instance.AddTab(nameof(ChzzkChat), "ChzzkChat.UI.leftPanel.bsml", new LeftPanelController());
        public static void RemoveLeftPanel() => GameplaySetup.instance.RemoveTab(nameof(ChzzkChat));
    }
}
