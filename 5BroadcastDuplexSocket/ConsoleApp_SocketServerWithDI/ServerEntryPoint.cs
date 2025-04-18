using ConsoleApp_5BroadcastDuplexSocket_3SocketServerWithDI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_5BroadcastDuplexSocket_3SocketServerWithDI
{
    public class ServerEntryPoint
    {
        private readonly IBroadcastService _broadcastService;
        private readonly IClientHandler _clientHandler;

        public ServerEntryPoint(IBroadcastService broadcastService, IClientHandler clientHandler)
        {
            _broadcastService = broadcastService;
            _clientHandler = clientHandler;
        }

        public async Task RunAsync()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("서버 시작됨.");
            Console.WriteLine("서버 시작! 클라이언트 기다리는 중...");

            _ = Task.Run(_broadcastService.StartPushLoopAsync); // 서버 푸시

            while (true)
            {
                var tcpClient = await listener.AcceptTcpClientAsync();
                Console.WriteLine("클라이언트 연결됨.");
                _ = Task.Run(() => _clientHandler.HandleAsync(tcpClient));
            }
        }
    }
}
