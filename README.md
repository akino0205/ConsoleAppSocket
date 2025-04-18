# ConsoleAppSocket
## 프로젝트 목표
* 멀티쓰레드를 이용한 소켓 서버/클라이언트를 구현하며 공부
  * 멀티쓰레드 이해
  * 단방향/양방향 소켓통신 이해
  * 

## 기술명세
* .NET6 콘솔 앱
![NET6-512BD4](https://user-images.githubusercontent.com/58022014/172149484-7c18b217-6b49-4f3f-940a-4853ae26f462.svg)

## 기능 명세
### 기본 환경 셋팅
  * .NET6 셋팅
  * VS 2022 셋팅
  * git 및 github 연결
### 2. 프로젝트별 명세
   #### 1) ConsoleApp_1SimpleThread
   * Task를 활용한 심플한 멀티쓰레드 구현  
   #### 2) ConsoleApp_2BackgroundLoop_1WhileThread
  * 기본적인 while 루프 + Thread.Sleep
    - 특징: 간단하고 직관적
    - 추천 상황: 복잡하지 않은 작업, .NET Framework 호환
  * 큐 기반 작업 처리 (Queue-based Task Processing)
  * Thread-safe Queue => ConcurrentQueue
   #### 3) ConsoleApp_2BackgroundLoop_2AsyncTask
  * async Task + Task.Delay 기반 비동기 루프
    - 특징: 비동기 친화적, 자원 효율적
    - 추천 상황: .NET Core/5 이상, 비동기 I/O 작업
  * 큐 기반 작업 처리 (Queue-based Task Processing)  
  * Thread-safe Queue => ConcurrentQueue
   #### 4) ConsoleApp_3SimplexSocket_1SocketServer
  * 전체 구조<br>
    TcpListener를 사용해 소켓 서버를 만들고<br>
    클라이언트로부터 문자열을 받으면 큐(ConcurrentQueue)에 저장<br>
    비동기 백그라운드 루프가 주기적으로 큐를 확인해 메시지 처리<br>
  * 테스트 방법<br>
    콘솔 앱을 실행하면 포트 5000번에서 소켓 서버가 실행돼요.<br>
    telnet, nc, 또는 간단한 클라이언트로 테스트 가능:<br>
    ```> echo "Hello from client" | nc localhost 5000      ```
  * 추가 확장 가능<br>
    요청마다 Task로 작업 처리 (병렬화)<br>
    큐 대신 Channel<T> 또는 BlockingCollection<T>로 변경<br>
    요청 객체를 모델 클래스로 분리<br>
    로그 기록, 에러 핸들링 강화<br>
   #### 5) ConsoleApp_3SimplexSocket_2SocketClient

   #### 6) ConsoleApp_4DuplexSocket_1SocketServer

   #### 7) ConsoleApp_4DuplexSocket_2SocketClient

   #### 8) ConsoleApp_5BroadcastDuplexSocket_1SocketServer

   #### 9) ConsoleApp_5BroadcastDuplexSocket_2SocketClient

   #### 10) ConsoleApp_5BroadcastDuplexSocket_3SocketServerWithDI
  * 양방향으로 변경
  * 서버<br>
    클라이언트 연결을 다수 수용<br>
    클라이언트로부터 받은 메시지를 모든 클라이언트에게 브로드캐스트<br>
    필요하면 주기적으로 서버가 먼저 메시지를 클라이언트들에게 푸시<br>
  * 실행 순서<br>
    1. BroadcastServer 먼저 실행
    2. BroadcastClient 실행
    3. 클라이언트에서 메시지를 입력하면 서버에 전송
    4. 서버는 연결된 클라이언트에 브로드캐스트
    5. 클라이언트는 메시지를 큐에 저장 후 비동기로 처리<br>
  🗨️ 채팅방 서버/클라이언트<br>
  📣 알림 푸시 시스템<br>
  🎮 실시간 멀티플레이어 게임 서버<br>
  …로 확장 가능!
   #### 11) ConsoleApp_5BroadcastDuplexSocket_4SocketClientWithDI
   *  양방향 클라이언트<br>
      서버에 메시지 전송<br>
      서버가 보내는 브로드캐스트 또는 푸시 메시지 수신<br>
      받은 메시지를 큐에 넣고 처리<br>
   * 실행 순서
      1. BroadcastServer 먼저 실행
      2. BroadcastClient 실행
      3. 클라이언트에서 메시지를 입력하면 서버에 전송
      4. 서버는 연결된 클라이언트에 브로드캐스트
      5. 클라이언트는 메시지를 큐에 저장 후 비동기로 처리
   * 전체 흐름
 [User Input] <br>
   ↓<br>
[ClientEntryPoint] → RunAsync()<br>
   ↓<br>
[IClientConnection / ClientConnection]<br>
   ↳ Connect to server<br>
   ↳ Send/Receive messages<br>
   ↳ Queue incoming messages<br>
   ↓<br>
[IMessageProcessor / MessageProcessor]<br>
   ↳ Process and display messages<br>
🗨️ 채팅방 서버/클라이언트<br>
      📣 알림 푸시 시스템<br>
      🎮 실시간 멀티플레이어 게임 서버<br>
      …로 확장 가능!
