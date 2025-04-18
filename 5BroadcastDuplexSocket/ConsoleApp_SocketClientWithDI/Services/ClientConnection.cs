using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_5BroadcastDuplexSocket_4SocketClientWithDI.Services
{
    /*
     * - 서버와 TCP 연결: 서버와의 실제 TCP 연결을 관리
     * - 수신 및 전송 처리: 사용자 입력 전송, 서버 메시지 수신 처리
     * - 메시지 큐 사용: 수신한 메시지를 큐에 넣고 비동기로 처리
     */
    public class ClientConnection: IClientConnection
    {
        private readonly IMessageProcessor _processor;
        private readonly ConcurrentQueue<string> _messageQueue = new();
        private bool _running = true;

        private StreamWriter? _writer;
        private StreamReader? _reader;

        public ClientConnection(IMessageProcessor processor)
        {
            _processor = processor;
        }

        /// <summary>
        /// 서버와 TCP 연결: 서버와의 실제 TCP 연결을 관리
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public async Task ConnectAsync(string host, int port)
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(host, port);
            NetworkStream stream = client.GetStream();

            _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            _reader = new StreamReader(stream, Encoding.UTF8);

            _ = Task.Run(ReceiveLoopAsync);
            _ = Task.Run(ProcessQueueLoopAsync);
        }

        /// <summary>
        /// 전송 처리: 사용자 입력 전송
        /// </summary>
        public async Task RunAsync()
        {
            while (_running)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    _running = false;
                    break;
                }

                if (_writer != null)
                    await _writer.WriteLineAsync(input);
            }
        }

        /// <summary>
        /// 수신 처리
        /// </summary>
        private async Task ReceiveLoopAsync()
        {
            try
            {
                while (_running && _reader != null)
                {
                    var msg = await _reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        Console.WriteLine($"[서버 수신] {msg}");
                        _messageQueue.Enqueue(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"수신 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 메시지 큐 사용: 수신한 메시지를 큐에 넣고 비동기로 처리
        /// </summary>
        private async Task ProcessQueueLoopAsync()
        {
            while (_running)
            {
                if (_messageQueue.TryDequeue(out var msg))
                {
                    await _processor.ProcessAsync(msg);
                }
                else
                {
                    await Task.Delay(300);
                }
            }
        }
    }
}
