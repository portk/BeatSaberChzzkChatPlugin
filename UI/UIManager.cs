using BeatSaberMarkupLanguage.Settings;

namespace ChzzkChat.UI
{
    internal class UIManager
    {
        private static SettingsController SettingsMenuInstance { get; set; }

        public static void AddSettingsMenu()
        {
            BSMLSettings.instance.AddSettingsMenu(nameof(ChzzkChat), "ChzzkChat.UI.settings.bsml", new SettingsController());
        }

        public static void RemoveSettingsMenu()
        {
            BSMLSettings.instance.RemoveSettingsMenu(SettingsMenuInstance);
        }
    }
}
