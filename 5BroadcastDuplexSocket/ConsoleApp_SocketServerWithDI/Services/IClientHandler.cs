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
    public interface IClientHandler
    {
        Task HandleAsync(TcpClient client);
    }
}
