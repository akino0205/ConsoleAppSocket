using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_5BroadcastDuplexSocket_3SocketServerWithDI.Services
{
    /// <summary>
    /// 클라이언트 연결 처리 로직
    /// </summary>
    public class ClientHandler : IClientHandler
    {
        private readonly IBroadcastService _broadcastService;

        public ClientHandler(IBroadcastService broadcastService)
        {
            _broadcastService = broadcastService;
        }

        public async Task HandleAsync(TcpClient client)
        {
            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            _broadcastService.AddClient(writer);

            try
            {
                while (true)
                {
                    var message = await reader.ReadLineAsync();
                    if (message == null) break;

                    Console.WriteLine($"[서버] 수신: {message}");
                    _broadcastService.BroadcastMessage($"[브로드캐스트] {message}");
                }
            }
            finally
            {
                _broadcastService.RemoveClient(writer);
            }
        }
    }
}
