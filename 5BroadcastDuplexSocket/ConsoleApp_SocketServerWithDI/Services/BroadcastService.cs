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
    public class BroadcastService : IBroadcastService
    {
        private readonly List<StreamWriter> _clients = new();
        private readonly object _lock = new();

        public void AddClient(StreamWriter writer)
        {
            lock (_lock) { _clients.Add(writer); }
        }

        public void RemoveClient(StreamWriter writer)
        {
            lock (_lock) { _clients.Remove(writer); }
        }

        public void BroadcastMessage(string message)
        {
            lock (_lock)
            {
                foreach (var writer in _clients)
                {
                    try { writer.WriteLine(message); } catch { }
                }
            }
        }

        public async Task StartPushLoopAsync()
        {
            int count = 1;
            while (true)
            {
                await Task.Delay(10000);
                BroadcastMessage($"[공지] 서버 자동 푸시 #{count++}");
            }
        }
    }
}
