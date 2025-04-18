using ConsoleApp_5BroadcastDuplexSocket_4SocketClientWithDI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_5BroadcastDuplexSocket_4SocketClientWithDI
{
    /*
     * - 클라이언트 실행 진입점: 앱 실행 시 RunAsync()를 통해 클라이언트를 실행
     * - 연결 & 메시지 루프 호출: 서버 연결 시도 및 메시지 송수신 처리 시작
     * - UI 입출력 담당: 사용자 입력을 받고 메시지를 전송함
     */
    public class ClientEntryPoint
    {
        private readonly IClientConnection _client;

        public ClientEntryPoint(IClientConnection client)
        {
            _client = client;
        }

        public async Task RunAsync()
        {
            await _client.ConnectAsync("127.0.0.1", 5000);
            Console.WriteLine("서버에 연결됨! 메시지를 입력하세요 (종료: Enter)");

            await _client.RunAsync();
        }
    }
}
