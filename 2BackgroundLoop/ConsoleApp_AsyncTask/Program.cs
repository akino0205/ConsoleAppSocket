using System.Collections.Concurrent;

internal class Program
{
    /*
     * async Task + Task.Delay 기반 비동기 루프
     * - 특징: 비동기 친화적, 자원 효율적
     * - 추천 상황: .NET Core/5 이상, 비동기 I/O 작업
     * 
     * 큐 기반 작업 처리 (Queue-based Task Processing) 
     * ConcurrentQueue => Thread-safe Queue
     */

    static ConcurrentQueue<string> taskQueue = new ConcurrentQueue<string>();
    static bool isRunning = true;

    static async Task Main()
    {
        taskQueue.Enqueue("Task A");
        taskQueue.Enqueue("Task B");

        // 백그라운드 비동기 루프 시작
        var processingTask = Task.Run(() => ProcessQueueLoopAsync());

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
        isRunning = false;

        await processingTask;
    }

    static async Task ProcessQueueLoopAsync()
    {
        while (isRunning)
        {
            if (taskQueue.TryDequeue(out var task))
            {
                Console.WriteLine($"Processing: {task}");
                // 작업 처리
            }
            else
            {
                // 비동기 대기 (CPU 덜 잡아먹음)
                await Task.Delay(500);
            }
        }

        Console.WriteLine("Worker stopped.");
    }
}