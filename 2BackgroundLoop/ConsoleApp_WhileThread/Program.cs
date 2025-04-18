using System.Collections.Concurrent;

internal class Program
{
    /*
     * 기본적인 while 루프 + Thread.Sleep
     * - 특징: 간단하고 직관적
     * - 추천 상황: 복잡하지 않은 작업, .NET Framework 호환
     * 
     * 큐 기반 작업 처리 (Queue-based Task Processing) 
     * ConcurrentQueue => Thread-safe Queue
     */

    static ConcurrentQueue<string> taskQueue = new ConcurrentQueue<string>();
    static bool isRunning = true;

    static void Main()
    {
        // 작업 추가 예시
        taskQueue.Enqueue("Task A");
        taskQueue.Enqueue("Task B");

        // 백그라운드 루프 시작
        Thread workerThread = new Thread(ProcessQueueLoop);
        workerThread.IsBackground = true;
        workerThread.Start();

        // 콘솔 앱 대기
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
        isRunning = false;
    }

    static void ProcessQueueLoop()
    {
        while (isRunning)
        {
            if (taskQueue.TryDequeue(out var task))
            {
                Console.WriteLine($"Processing: {task}");
                // 작업 처리 로직 (예: 비동기 I/O, DB 처리 등)
            }
            else
            {
                // 작업 없으면 잠깐 대기 (CPU 낭비 방지)
                Thread.Sleep(500);
            }
        }

        Console.WriteLine("Worker stopped.");
    }
}