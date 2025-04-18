using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_5BroadcastDuplexSocket_3SocketServerWithDI.Services
{
    /// <summary>
    /// 메시지 브로드캐스트 및 푸시 로직
    /// </summary>
    public interface IBroadcastService
    {
        void AddClient(StreamWriter writer);
        void RemoveClient(StreamWriter writer);
        void BroadcastMessage(string message);
        Task StartPushLoopAsync();
    }
}
