using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    /*
     * [[구조 설명]]
     * 클라이언트는 소켓을 통해 서버와 연결
     * 서버로부터 데이터를 수신하면 → 큐(ConcurrentQueue)에 저장
     * 비동기로 백그라운드 워커가 큐를 주기적으로 확인하고 처리
     * 
     * [[테스트 방법]]
     * 서버는 포트 5000에서 텍스트 메시지를 전송해줘야 해요. 
     * 테스트용 서버는 간단히 TcpListener로 만들거나, netcat, telnet, 또는 아래처럼 만들 수 있어요:
     * 
     * [[확장 아이디어]]
     * 클라이언트가 메시지를 보낼 수도 있게 만들기 (양방향)
     * 메시지를 JSON 등 구조화된 포맷으로 변환 후 큐 처리
     * 처리 결과를 로그로 남기거나 UI로 전송
     */

    static ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
    static bool isRunning = true;

    static async Task Main()
    {
        // 서버에 연결하고 메시지 수신 시작
        _ = Task.Run(() => ConnectAndReceiveAsync("127.0.0.1", 5000));

        // 백그라운드 큐 처리 루프 시작
        _ = Task.Run(ProcessQueueLoopAsync);

        Console.WriteLine("클라이언트 실행 중... 종료하려면 Enter를 누르세요.");
        Console.ReadLine();
        isRunning = false;
    }

    static async Task ConnectAndReceiveAsync(string host, int port)
    {
        try
        {
            using TcpClient client = new TcpClient();
            await client.ConnectAsync(host, port);
            Console.WriteLine("서버에 연결됨.");

            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8);

            while (isRunning)
            {
                string? message = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine($"서버로부터 수신: {message}");
                    messageQueue.Enqueue(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"연결 또는 수신 중 오류 발생: {ex.Message}");
        }
    }

    static async Task ProcessQueueLoopAsync()
    {
        while (isRunning)
        {
            if (messageQueue.TryDequeue(out var message))
            {
                Console.WriteLine($"👉 큐에서 꺼낸 메시지 처리: {message}");
                // 실제 처리 로직
                await Task.Delay(100); // 모의 처리 시간
            }
            else
            {
                await Task.Delay(300); // 큐 비어있을 때 잠깐 대기
            }
        }

        Console.WriteLine("백그라운드 처리 루프 종료");
    }
}