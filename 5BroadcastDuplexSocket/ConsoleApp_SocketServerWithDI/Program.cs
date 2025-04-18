using ConsoleApp_5BroadcastDuplexSocket_3SocketServerWithDI;
using ConsoleApp_5BroadcastDuplexSocket_3SocketServerWithDI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/*
 * - 앱 시작점:	콘솔앱의 진입점. HostBuilder를 통해 DI 컨테이너를 구성하고 앱 실행
 * - DI 활성화:	서비스들을 등록하고 ServerEntryPoint 실행
 */
internal class Program
{
    /*
     * 양방향으로 변경
     * [[서버]]
     * 클라이언트 연결을 다수 수용
     * 클라이언트로부터 받은 메시지를 모든 클라이언트에게 브로드캐스트
     * 필요하면 주기적으로 서버가 먼저 메시지를 클라이언트들에게 푸시
     * 
     * [[실행 순서]]
     * 1. BroadcastServer 먼저 실행
     * 2. BroadcastClient 실행
     * 3. 클라이언트에서 메시지를 입력하면 서버에 전송
     * 4. 서버는 연결된 클라이언트에 브로드캐스트
     * 5. 클라이언트는 메시지를 큐에 저장 후 비동기로 처리
     * 
     * 🗨️ 채팅방 서버/클라이언트
     * 📣 알림 푸시 시스템
     * 🎮 실시간 멀티플레이어 게임 서버
     * …로 확장 가능해요!
     */

    static async Task Main()
    {
        Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                //DI 컨테이너 구성 및 서비스(인터페이스/ 구현 클래스) 등록
                services.AddSingleton<IBroadcastService, BroadcastService>();
                services.AddTransient<IClientHandler, ClientHandler>();
                services.AddSingleton<ServerEntryPoint>();
            })
            .Build()
            .Services.GetRequiredService<ServerEntryPoint>()
            .RunAsync() //소켓 클라 시작
            .GetAwaiter()
            .GetResult();
    }
}