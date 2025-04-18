using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_5BroadcastDuplexSocket_4SocketClientWithDI.Services
{
    /*
     * - 수신 메시지 처리: 수신된 메시지를 화면에 출력하거나 다른 처리를 수행
     * - 비즈니스 로직 담당: 이후 특정 메시지별 분기처리, DB 저장 등도 여기서 확장 가능
     */
    public class MessageProcessor: IMessageProcessor
    {
        public Task ProcessAsync(string message)
        {
            Console.WriteLine($"👉 처리 중: {message}");
            return Task.Delay(100); // 처리 시간 시뮬레이션
        }
    }
}
