using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_5BroadcastDuplexSocket_4SocketClientWithDI.Services
{
    /*
     * - 메시지 처리 인터페이스: 메시지 처리 로직에 대한 추상화
     * - 확장성 제공: 로깅, 저장소, 필터 등 다양한 처리 로직으로 교체 가능하게 함
     */
    public interface IMessageProcessor
    {
        Task ProcessAsync(string message);
    }
}
