using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.Text;

internal class Program
{
    /*
     * [[전체 구조]]
     * TcpListener를 사용해 소켓 서버를 만들고
     * 클라이언트로부터 문자열을 받으면 큐(ConcurrentQueue)에 저장
     * 비동기 백그라운드 루프가 주기적으로 큐를 확인해 메시지 처리
     * 
     * [[테스트 방법]]
     * 콘솔 앱을 실행하면 포트 5000번에서 소켓 서버가 실행돼요.
     * telnet, nc, 또는 간단한 클라이언트로 테스트 가능:
     * > echo "Hello from client" | nc localhost 5000
     * 
     * [[추가 확장 가능]]
     * 요청마다 Task로 작업 처리 (병렬화)
     * 큐 대신 Channel<T> 또는 BlockingCollection<T>로 변경
     * 요청 객체를 모델 클래스로 분리
     * 로그 기록, 에러 핸들링 강화
     */

    static ConcurrentQueue<string> requestQueue = new ConcurrentQueue<string>();
    static bool isRunning = true;

    static async Task Main()
    {
        // 소켓 리스너 시작
        _ = Task.Run(StartSocketServer);

        // 백그라운드 작업 처리 루프 시작
        _ = Task.Run(ProcessQueueLoopAsync);

        Console.WriteLine("서버 실행 중... 종료하려면 Enter를 누르세요.");
        Console.ReadLine();
        isRunning = false;
    }

    static async Task StartSocketServer()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 5000);
        listener.Start();
        Console.WriteLine("소켓 서버 시작: 포트 5000");

        while (isRunning)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            _ = Task.Run(() => HandleClientAsync(client));
        }

        listener.Stop();
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            string? message = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine($"클라이언트 메시지 수신: {message}");
                requestQueue.Enqueue(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"클라이언트 처리 중 예외 발생: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    static async Task ProcessQueueLoopAsync()
    {
        while (isRunning)
        {
            if (requestQueue.TryDequeue(out var request))
            {
                Console.WriteLine($"👉 처리 중: {request}");
                // 여기서 작업 처리 로직 추가 가능
                await Task.Delay(100); // 가짜 처리 시간
            }
            else
            {
                await Task.Delay(500); // 큐가 비어있으면 대기
            }
        }

        Console.WriteLine("백그라운드 처리 루프 종료");
    }

}