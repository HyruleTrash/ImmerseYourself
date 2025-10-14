using System.Net;
using System.Net.Sockets;

namespace ImmerseYourselfServer;

public class Server
{
    private static int MaxClients { get; set; }
    private static TcpListener TcpListener { get; set; }
    private const int port = 4377;
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int fromClient, Packet packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    public static void Start(int maxClients)
    {
        MaxClients = maxClients;
        
        Console.WriteLine("Starting server...");
        InitializeServerData();
        
        TcpListener = new TcpListener(IPAddress.Any, port);
        TcpListener.Start();
        TcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        
        Console.WriteLine($"Server started on port {port}");
    }

    private static void TCPConnectCallback(IAsyncResult ar)
    {
        TcpClient _client = TcpListener.EndAcceptTcpClient(ar);
        
        TcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);
        Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

        foreach (var (key, client) in clients)
        {
            if (client.tcp.socket != null) continue;
            client.tcp.Connect(_client);
            return;
        }
        
        Console.WriteLine($"Server is at full capacity");
    }

    private static void InitializeServerData()
    {
        for (var i = 0; i < MaxClients; i++)
        {
            clients.Add(i, new Client(i));
        }
        Console.WriteLine($"Lobby of size {clients.Count} has been created!");
        
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeRecieved}
        };
        Console.WriteLine("Packet handlers have been registered.");
    }
}