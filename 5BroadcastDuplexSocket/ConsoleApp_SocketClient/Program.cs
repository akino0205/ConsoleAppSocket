using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    /*
     * 양방향으로 변경
     * [[클라이언트]]
     * 서버에 메시지 전송
     * 서버가 보내는 브로드캐스트 또는 푸시 메시지 수신
     * 받은 메시지를 큐에 넣고 처리
     * 
     * [[실행 순서]]
     * 1. BroadcastServer.cs 실행 → 서버 시작됨
     * 2. BroadcastClient.cs 여러 개 실행 → 여러 클라이언트 연결
     * 3. 한 클라이언트에서 메시지 입력 → 서버 수신 → 모든 클라이언트에 브로드캐스트
     * 4. 서버는 10초마다 공지 메시지도 자동 푸시
     * 
     * 🗨️ 채팅방 서버/클라이언트
     * 📣 알림 푸시 시스템
     * 🎮 실시간 멀티플레이어 게임 서버
     * …로 확장 가능해요!
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
                await writer.WriteLineAsync(input);
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
                Console.WriteLine($"[서버 수신] {msg}");
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
    }
}