using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Text;

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

    static List<StreamWriter> connectedClients = new List<StreamWriter>();
    static object lockObj = new object();
    static bool isRunning = true;

    static async Task Main()
    {
        // 소켓 리스너 시작
        _ = Task.Run(StartSocketServer);

        // 서버에서 주기적으로 공지 푸시
        _ = Task.Run(ServerPushLoop);

        //Console.WriteLine("서버 실행 중... 종료하려면 Enter를 누르세요.");
        //Console.ReadLine();
        //isRunning = false;
    }

    static async Task StartSocketServer()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 5000);
        listener.Start();
        Console.WriteLine("소켓 서버 시작: 포트 5000");
        Console.WriteLine("서버 시작! 클라이언트 기다리는 중...");

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
        StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        lock (lockObj)
        {
            connectedClients.Add(writer);
        }

        try
        {
            while (true)
            {
                string? message = await reader.ReadLineAsync();
                if (message == null) break;

                Console.WriteLine($"[서버] 클라이언트 메시지 수신: {message}");

                // 모든 클라이언트에게 브로드캐스트
                BroadcastMessage($"[브로드캐스트] {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"클라이언트 처리 중 오류: {ex.Message}");
        }
        finally
        {
            lock (lockObj)
            {
                connectedClients.Remove(writer);
            }
        }
    }

    static void BroadcastMessage(string message)
    {
        lock (lockObj)
        {
            foreach (var writer in connectedClients)
            {
                try
                {
                    writer.WriteLine(message);
                }
                catch
                {
                    // 무시 (연결 끊긴 경우 등)
                }
            }
        }
    }

    static async Task ServerPushLoop()
    {
        int count = 1;
        while (true)
        {
            await Task.Delay(10000); // 10초마다
            BroadcastMessage($"[공지] 서버에서 자동 전송하는 메시지 #{count++}");
        }
    }
}