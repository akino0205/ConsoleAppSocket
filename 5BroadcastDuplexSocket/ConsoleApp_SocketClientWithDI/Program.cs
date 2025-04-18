using ConsoleApp_5BroadcastDuplexSocket_4SocketClientWithDI.Services;
using ConsoleApp_5BroadcastDuplexSocket_4SocketClientWithDI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

/*
 * - 앱 시작점:	콘솔앱의 진입점. HostBuilder를 통해 DI 컨테이너를 구성하고 앱 실행
 * - DI 활성화:	서비스들을 등록하고 ClientEntryPoint 실행
 */
internal class Program
{
    /* [[양방향 클라이언트]]
     * 서버에 메시지 전송
     * 서버가 보내는 브로드캐스트 또는 푸시 메시지 수신
     * 받은 메시지를 큐에 넣고 처리
     */
    /* [[실행 순서]]
     * 1. BroadcastServer 먼저 실행
     * 2. BroadcastClient 실행
     * 3. 클라이언트에서 메시지를 입력하면 서버에 전송
     * 4. 서버는 연결된 클라이언트에 브로드캐스트
     * 5. 클라이언트는 메시지를 큐에 저장 후 비동기로 처리
     */
    /* [[전체 흐름]]
 [User Input] 
   ↓
[ClientEntryPoint] → RunAsync()
   ↓
[IClientConnection / ClientConnection]
   ↳ Connect to server
   ↳ Send/Receive messages
   ↳ Queue incoming messages
   ↓
[IMessageProcessor / MessageProcessor]
   ↳ Process and display messages
     */
    /* 확장 가능성
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
                services.AddSingleton<IClientConnection, ClientConnection>();
                services.AddSingleton<IMessageProcessor, MessageProcessor>();
                services.AddSingleton<ClientEntryPoint>();
            })
            .Build()
            .Services.GetRequiredService<ClientEntryPoint>()
            .RunAsync()
            .GetAwaiter()
            .GetResult();
    }
}