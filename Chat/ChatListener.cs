using BeatSaberMarkupLanguage.Settings;
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

        public ChatListener()
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

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
                        new JProperty("uid", userInformation.uId == "" ? null : userInformation.uId),
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
                //ThreadPool.QueueUserWorkItem(Send);
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
                            ParseChat(serverMsg);
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log.Error(serverMsg);
                    Plugin.Log.Error(e.Message);
                }
            }

        }

        // 미구현
        // 사용자가 채팅을 입력해서 보내는 부분
        // 따로 만들고 떼온거라 서식이 이 프로그램에 알맞지 않다.
        // 폼에서 입력받고 전송의사를 받아서 발송하는 형태로 변경해야함
        //private void Send(Object obj)
        //{
        //    string msg = (string)obj;
        //    if (msg != null)
        //    {
        //        ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
        //        client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        //    }
        //}

        public async void CloseClient()
        {
            await client.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
        }

        private async void Send(string msg)
        {
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void ParseChat(string ReceiveString)
        {
            JObject ReceiveObject = JObject.Parse(ReceiveString);

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

                        Plugin.Log.Info($"{Nickname} {Msg}");
                    }
                    // anonymous donate
                    else if (IsAnonymous == true)
                    {
                        string Msg = (string)chat["msg"];
                        int PayAmount = (int)JObject.Parse((string)chat["extras"])["payAmount"];

                        Plugin.Log.Info($"anonymous donate {PayAmount}w {Msg}");
                    }
                    // donate
                    else if (IsAnonymous == false)
                    {
                        string Nickname = (string)JObject.Parse((string)chat["profile"])["nickname"];
                        string Msg = (string)chat["msg"];
                        int PayAmount = (int)JObject.Parse((string)chat["extras"])["payAmount"];

                        Plugin.Log.Info($"{Nickname} donate {PayAmount} {Msg}");
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
                    Plugin.Log.Info(
                        $"Mission {missionText} |{totalPayAmount}w| time:{durationTime} {nickname}(and {participationCount} others)"
                        );
                }

            }
        }

        private void ThreadingGetRequest(object songCode)
        {
            //requestListControl.GetRequest((string)songCode);
        }
    }
}
