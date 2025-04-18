using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_5BroadcastDuplexSocket_4SocketClientWithDI.Services
{
    /*
     * - 클라이언트 연결 인터페이스: ClientConnection 구현체에 대한 추상화
     * - 유연한 테스트와 DI용: 테스트나 다른 구현으로 교체할 수 있게 도와줌
     */
    public interface IClientConnection
    {
        Task ConnectAsync(string host, int port);
        Task RunAsync();
    }
}
