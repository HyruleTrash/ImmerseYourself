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

        bool found = false;
        foreach (var (key, client) in clients)
        {
            if (client.tcp.socket != null) continue;
            client.tcp.Connect(_client);
            found = true;
            return;
        }
        if (found)
            return;
        Console.WriteLine($"Server is at full capacity");
        
        Client tempClient = new Client(-1);
        tempClient.tcp.Connect(_client);
        ServerSend.ServerFull(tempClient.tcp);
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
            {(int)ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived},
            {(int)ClientPackets.MiniGameOver, ServerHandle.MiniGameOver}
        };
        Console.WriteLine("Packet handlers have been registered.");
    }

    private static bool IsServerFull()
    {
        bool full = true;
        foreach (var (key, client) in clients)
        {
            if (client.tcp.socket != null) continue;
            full = false;
        }
        return full;
    }

    private static int lastPickedId = -1;
    public static bool isPlaying = false;
    public static void StartGame(MiniGames lastPlayedMiniGame = 0)
    {
        var isServerFull = IsServerFull();
        Console.WriteLine($"Server full status: {isServerFull}");
        if (!isServerFull || isPlaying)
            return;
        Console.WriteLine("Starting game...");
        var pickedEntry = clients.GetRandomEntry();

        while (pickedEntry.HasValue == false || lastPickedId == pickedEntry.Value.Key)
        {
            pickedEntry = clients.GetRandomEntry();
            if (pickedEntry.HasValue && lastPickedId != pickedEntry.Value.Key)
                break;
        }

        lastPickedId = pickedEntry.Value.Key;
        ServerSend.StartMiniGame(pickedEntry.Value.Key, lastPlayedMiniGame);
        isPlaying = true;
    }
}