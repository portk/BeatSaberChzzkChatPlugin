using ChzzkChat.Configuration;
using ChzzkChat.SongRequest;
using ChzzkChat.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChzzkChat.Chat
{
    class ChatListener
    {
        private ClientWebSocket client = new ClientWebSocket();
        private Uri uri;
        private readonly string pingMsg = "{\"ver\":\"2\",\"cmd\":0}";
        private readonly string pongMsg = "{\"ver\":\"2\",\"cmd\":10000}";
        private Random rand = new Random();
        private RequestListControl requestListControl = new RequestListControl();

        public ChatListener()
        {
            int workerThreads = 1, completionPortThreads = 1;

            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            ThreadPool.SetMinThreads(workerThreads+1, completionPortThreads+1);
            ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
            ThreadPool.SetMaxThreads(workerThreads+5, completionPortThreads+5);


            int id = rand.Next(1, 11);
            uri = new Uri($"wss://kr-ss{id}.chat.naver.com/chat");
        }

        public async Task Init()
        {
            GetChannelInfo userInformation = new GetChannelInfo();
            
            try
            {
                await client.ConnectAsync(uri, CancellationToken.None);

                JObject connectObj = new JObject(
                    new JProperty("ver", "2"),
                    new JProperty("cmd", 100),
                    new JProperty("svcid", "game"),
                    new JProperty("cid", userInformation.chatChannelId),
                    new JProperty("bdy", new JObject(
                        new JProperty("uid", userInformation.uId == "" ? "" : userInformation.uId),
                        new JProperty("devType", 2001),
                        new JProperty("accTkn", userInformation.accessToken),
                        new JProperty("auth", (userInformation.uId != "") ? "SEND" : "READ")
                        )
                    ),
                    new JProperty("tid", 1)
                    ) ;

                // first touch
                string jsonString = connectObj.ToString();
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonString));
                await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

                ThreadPool.QueueUserWorkItem(Listen);
            }
            catch (Exception e)
            {
                Plugin.Log.Error(e.Message);
            }
        }

        private async void Listen(Object obj)
        {
            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[16384]);

            if (client.State == WebSocketState.Open)
            {
                Plugin.Log.Debug("Socket Link Success");
            }
            else
            {
                Plugin.Log.Error("Socket Link Fail");
            }

            while (client.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await client.ReceiveAsync(bytesReceived, CancellationToken.None);
                string serverMsg = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

                try
                {
                    if (serverMsg == pingMsg) Send(pongMsg);
                    else
                    {
                        ThreadPool.QueueUserWorkItem(ParseChat, serverMsg);
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.Error(serverMsg);
                    Plugin.Log.Error(e.Message);
                }
            }

        }

        public async void CloseClient()
        {
            await client.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
        }

        private async void Send(string msg)
        {
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void ParseChat(object ReceiveString)
        {
            JObject ReceiveObject = JObject.Parse((string)ReceiveString);

            if (ReceiveObject["bdy"].Type == JTokenType.Array)
            {
                foreach (var chat in ReceiveObject["bdy"])
                {
                    var IsAnonymous = (bool?)JObject.Parse((string)chat["extras"])["isAnonymous"];

                    // common chat
                    if (IsAnonymous == null)
                    {
                        string Nickname = (string)JObject.Parse((string)chat["profile"])["nickname"];
                        string Msg = (string)chat["msg"];

                        Plugin.Log.Debug($"{Nickname} {Msg}");

                        if(PluginConfig.Instance.RequestQueOpen && Msg.StartsWith($"{PluginConfig.Instance.RequestCommand} "))
                        {
                            Msg = Msg.Remove(0, PluginConfig.Instance.RequestCommand.Length + 1);

                            if (Msg.Length > 0)
                            {
                                int idx = Msg.IndexOf(" ");

                                // prevent like !bsr 1 hi! -> read 1
                                if (idx == -1)
                                {
                                    ThreadPool.QueueUserWorkItem(ThreadingGetRequest,Msg);
                                }
                                else
                                {
                                    ThreadPool.QueueUserWorkItem(ThreadingGetRequest, Msg);
                                }
                            }
                        }
                    }
                    // anonymous donate
                    else if (IsAnonymous == true)
                    {
                        string Msg = (string)chat["msg"];
                        int PayAmount = (int)JObject.Parse((string)chat["extras"])["payAmount"];

                        Plugin.Log.Debug($"anonymous donate {PayAmount}w {Msg}");
                    }
                    // donate
                    else if (IsAnonymous == false)
                    {
                        string Nickname = (string)JObject.Parse((string)chat["profile"])["nickname"];
                        string Msg = (string)chat["msg"];
                        int PayAmount = (int)JObject.Parse((string)chat["extras"])["payAmount"];

                        Plugin.Log.Debug($"{Nickname} donate {PayAmount} {Msg}");
                    }
                }
            }
            // mission
            else if (ReceiveObject["bdy"].Type == JTokenType.Object)
            {
                var durationTime = (int?)ReceiveObject["bdy"]["durationTime"];
                var totalPayAmount = (int?)ReceiveObject["bdy"]["totalPayAmount"];
                var missionText = (string)ReceiveObject["bdy"]["missionText"];
                var nickname = (string)ReceiveObject["bdy"]["nickname"];
                var participationCount = (string)ReceiveObject["bdy"]["participationCount"];

                if (totalPayAmount != null)
                {
                    Plugin.Log.Debug(
                        $"Mission {missionText} |{totalPayAmount}w| time:{durationTime} {nickname}(and {participationCount} others)"
                        );
                }

            }
        }

        private void ThreadingGetRequest(object songCode)
        {
            requestListControl.GetRequest((string)songCode);
        }
    }
}
