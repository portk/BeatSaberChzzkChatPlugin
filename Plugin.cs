using ChzzkChat.Chat;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using IPAConfig = IPA.Config.Config;
using ChzzkChat.Configuration;
using ChzzkChat.UI;

namespace ChzzkChat
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        [Init]
        public void Init(IPALogger logger, IPAConfig iPAConfig, PluginMetadata pluginMetadata)
        {
            Instance = this;
            Log = logger;

            Log.Info("ChzzkChat initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config

        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }

        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Log.Debug("OnApplicationStart");
            new GameObject("ChzzkChatController").AddComponent<ChzzkChatController>();

            GetChannelInfo channelInfo = new GetChannelInfo();
            ChatListener chatListener = new ChatListener();
            UIManager.AddSettingsMenu();
            UIManager.AddLeftPanel();

            _ = chatListener.Init();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            UIManager.RemoveSettingsMenu();
            UIManager.RemoveLeftPanel();

            Log.Debug("OnApplicationQuit");
        }
    }
}
