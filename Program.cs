using System.Net.Sockets;
using System.Text;

namespace BroadcastClient;

class Program {
    private const string serverIp = "localhost";
    private const int port = 5035;
    private static bool isRunning = true;

    static async Task Main(string[] args) {
        using TcpClient client = new TcpClient();
        await client.ConnectAsync(serverIp, port);
        Console.WriteLine("Connected to server");

        NetworkStream stream = client.GetStream();
        _ = Task.Run(() => ReceiveMessage(stream));

        while (true) {
            string messageSending = Console.ReadLine();

            if (string.IsNullOrEmpty(messageSending)) 
                continue;           

            byte[] buffer = Encoding.UTF8.GetBytes(messageSending);
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
    }

    private static async void ReceiveMessage(NetworkStream stream) {
        byte[] buffer = new byte[1024];

        while (true) {
            int numberBytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (numberBytesRead == 0) {
                Console.WriteLine("Server disconnected");
                break;
            }

            string messageReceiving = Encoding.UTF8.GetString(buffer, 0, numberBytesRead);
            Console.WriteLine($"Received: {messageReceiving}");
        }
    }
}