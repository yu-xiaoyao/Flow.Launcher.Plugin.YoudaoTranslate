using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Flow.Launcher.Plugin.YoudaoTranslate.Youdao
{
    class WebsocketUtil
    {
        // 初始化websocket连接
        private static ClientWebSocket? client;

        public static Task initConnection(string url)
        {
            var client = new ClientWebSocket();
            client.ConnectAsync(new Uri(url), CancellationToken.None).Wait();
            WebsocketUtil.client = client;
            // 监听返回结果
            return messageHandler(client);
        }

        public static Task initConnectionWithParams(string url, Dictionary<String, String[]> paramsMap)
        {
            StringBuilder content = new StringBuilder();
            content.Append(url);
            if (paramsMap != null)
            {
                content.Append("?");
                int i = 0;
                foreach (var p in paramsMap)
                {
                    foreach (var v in p.Value)
                    {
                        if (i > 0)
                        {
                            content.Append("&");
                        }

                        content.AppendFormat("{0}={1}", p.Key, HttpUtility.UrlEncode(v));
                        i++;
                    }
                }
            }

            return initConnection(content.ToString());
        }

        // 发送text message
        public static void sendTextMessage(string textMsg)
        {
            if (client == null || client.State != WebSocketState.Open)
            {
                throw new Exception("websocket connection not established");
            }

            byte[] bytes = Encoding.UTF8.GetBytes(textMsg);
            client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
            Console.WriteLine("send text message: " + textMsg);
        }

        // 发送binary message
        public static void sendBinaryMessage(byte[] binaryMessage)
        {
            if (client == null || client.State != WebSocketState.Open)
            {
                throw new Exception("websocket connection not established");
            }

            client.SendAsync(new ArraySegment<byte>(binaryMessage), WebSocketMessageType.Binary, true,
                CancellationToken.None);
            Console.WriteLine("send binary message length: " + binaryMessage.Length);
        }


        public static async Task messageHandler(ClientWebSocket client)
        {
            string msg = "";
            while (true)
            {
                while (client == null || client.State != WebSocketState.Open)
                {
                    Thread.Sleep(1000);
                }

                var buffer = new byte[1024 * 1024];
                var message = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (message.MessageType == WebSocketMessageType.Text)
                {
                    var res = Encoding.UTF8.GetString(buffer, 0, message.Count);
                    // 消息过大时可能接收不全
                    if (!res.Contains("\"errorCode\""))
                    {
                        msg += res;
                        continue;
                    }

                    msg += res;
                    Console.WriteLine("received text message: " + msg);
                    msg = "";
                    if (!res.Contains("\"errorCode\":\"0\""))
                    {
                        client.Dispose();
                        break;
                    }
                }
                else if (message.MessageType == WebSocketMessageType.Binary)
                {
                    Console.WriteLine("received binary message length: " + message.Count);
                }
                else if (message.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("connection closed.");
                    break;
                }
            }
        }
    }
}