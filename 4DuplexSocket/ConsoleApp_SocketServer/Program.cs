using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Text;

internal class Program
{
    /*
     * 양방향으로 변경
     * 서버: 여러 클라이언트와 연결하고 메시지를 주고받음
     * 
     * 지금 구조는 클라이언트가 먼저 보내고 서버가 응답하는 패턴
     * 클라이언트와 서버가 서로 읽고 쓰는 루프를 독립적으로 운영하고 있으므로, 상호 통신 가능한 구조입니다.
     */

    static ConcurrentQueue<string> requestQueue = new ConcurrentQueue<string>();
    static bool isRunning = true;

    static async Task Main()
    {
        // 소켓 리스너 시작
        _ = Task.Run(StartSocketServer);
    }

    static async Task StartSocketServer()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 5000);
        listener.Start();
        Console.WriteLine("소켓 서버 시작: 포트 5000");

        while (isRunning)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("클라이언트 연결됨!");

            _ = Task.Run(() => HandleClientAsync(client));
        }

        //listener.Stop();
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        while (true)
        {
            string? message = await reader.ReadLineAsync();
            if (message == null) break;

            Console.WriteLine($"[서버] 수신: {message}");

            // 클라이언트에게 응답
            string response = $"[서버 응답] {message}"; // 서버 → 클라이언트
            await writer.WriteLineAsync(response);
        }

        Console.WriteLine("클라이언트 연결 종료됨.");
    }
}