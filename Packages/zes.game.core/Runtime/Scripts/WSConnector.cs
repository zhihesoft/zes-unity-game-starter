using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zes
{
    /// <summary>
    /// WebSocket connector
    /// </summary>
    public class WSConnector
    {
        public bool connected => socket != null && socket.State == WebSocketState.Open;
        public Action<string> onMessage;

        private ClientWebSocket socket;
        private Logger logger = Logger.GetLogger<WSConnector>();

        public async Task<bool> Open(string url, string token)
        {
            if (socket != null)
            {
                logger.Error("socket is not null, please close connector at first");
                return false;
            }

            socket = new ClientWebSocket();
            socket.Options.SetRequestHeader("Authorization", $"Bearer {token}");
            var uri = new Uri(url);
            try
            {
                await socket.ConnectAsync(uri, CancellationToken.None);
                MessageLoop(socket);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"socket connect failed: {ex.Message}");
                socket.Dispose();
                socket = null;
                return false;
            }
        }

        public async Task Close()
        {
            if (socket != null)
            {
                socket.Abort();
                await socket.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
                socket = null;
            }
        }

        public async Task Send(string message)
        {
            if (socket == null || socket.State != WebSocketState.Open)
            {
                logger.Error("Send failed: websocket is not connected. ");
                return;
            }
            var bytes = Util.utf8WithoutBOM.GetBytes(message);
            await socket.SendAsync(new ReadOnlyMemory<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async void MessageLoop(WebSocket ws)
        {
            byte[] buffer = new byte[1024 * 32];
            while (ws == socket && ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.Count > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer);
                    onMessage?.Invoke(message);
                }
            }
            logger.Info("ws message loop end.");
        }

    }
}
