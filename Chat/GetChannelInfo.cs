using ChzzkChat.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Text;

namespace ChzzkChat.Chat
{
    class GetChannelInfo
    {
        WebClient client = new WebClient();
        public string uId { get; set; } = "";
        public string chatChannelId { get; set; } = "";
        public string accessToken { get; set; } = "";
        public string extraToken { get; set; } = "";

        // Set Request Header
        public GetChannelInfo()
        {
            client.Headers.Clear();
            client.Headers.Add("Accept", "application/json");
            client.Encoding = Encoding.UTF8;

            SendRequest();
        }

        public void SendRequest()
        {
            // Get Channel Id from Settings
            string channelId = PluginConfig.Instance.ChannelId;

            try
            {
                string response = "";

                // Get User Information 
                response = client.DownloadString("https://comm-api.game.naver.com/nng_main/v1/user/getUserStatus");
                JObject myInfo = JObject.Parse(response);
                uId = (string)myInfo["content"]["userIdHash"];
                uId = uId ?? "";

                // Get Channel Information
                response = client.DownloadString($"https://api.chzzk.naver.com/polling/v2/channels/{channelId}/live-status");
                JObject liveStatus = JObject.Parse(response);
                chatChannelId = (string)liveStatus["content"]["chatChannelId"];

                // Get Access Token
                response = client.DownloadString($"https://comm-api.game.naver.com/nng_main/v1/chats/access-token?channelId={chatChannelId}&chatType=STREAMING");
                JObject getAccessToken = JObject.Parse(response);
                accessToken = (string)getAccessToken["content"]["accessToken"];
                extraToken = (string)getAccessToken["content"]["extraToken"];

                Plugin.Log.Debug("Channel Info Access Success");
            }
            catch (Exception ex)
            {
                Plugin.Log.Error("Channel Info Access Fail!");
                Plugin.Log.Error(ex.StackTrace);
            }
        }
    }
}
