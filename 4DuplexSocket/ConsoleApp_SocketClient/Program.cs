using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    /*
     * 양방향으로 변경
     * 클라이언트: 메시지 입력 → 서버 전송 / 응답 수신 → 큐에 저장 → 워커 처리
     * 
     * 지금 구조는 클라이언트가 먼저 보내고 서버가 응답하는 패턴
     * 클라이언트와 서버가 서로 읽고 쓰는 루프를 독립적으로 운영하고 있으므로, 상호 통신 가능한 구조입니다.
     */

    static ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
    static bool isRunning = true;

    static async Task Main()
    {

        // 서버에 연결하고 메시지 수신 시작
        _ = Task.Run(() => ConnectAndReceiveAsync("127.0.0.1", 5000));
        

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

            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            // 메시지 송신 & 수신 루프
            _ = Task.Run(() => SendLoopAsync(writer));
            _ = Task.Run(() => ReceiveLoopAsync(reader));
            _ = Task.Run(() => ProcessQueueLoopAsync());

        }
        catch (Exception ex)
        {
            Console.WriteLine($"연결 또는 수신 중 오류 발생: {ex.Message}");
        }
    }

    static async Task SendLoopAsync(StreamWriter writer)
    {
        while (isRunning)
        {
            Console.Write("보낼 메시지 입력: ");
            var input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                await writer.WriteLineAsync(input); // 클라이언트 → 서버
            }
        }
    }

    static async Task ReceiveLoopAsync(StreamReader reader)
    {
        while (isRunning)
        {
            var msg = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(msg))
            {
                Console.WriteLine($"[서버로부터 수신] {msg}");
                messageQueue.Enqueue(msg);
            }
        }
    }

    static async Task ProcessQueueLoopAsync()
    {
        while (isRunning)
        {
            if (messageQueue.TryDequeue(out var msg))
            {
                Console.WriteLine($"👉 처리 중: {msg}");
                await Task.Delay(100); // 모의 처리
            }
            else
            {
                await Task.Delay(300);
            }
        }

        Console.WriteLine("백그라운드 큐 처리 종료");
    }
}