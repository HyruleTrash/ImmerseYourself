namespace ImmerseYourselfServer
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            var temp = Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint;
        
            Console.WriteLine($"{temp}: connected successfully!");
            if (clientIdCheck != fromClient)
                Console.WriteLine($"{temp} has assumed the wrong client ID. monitor ID: {clientIdCheck}");

            Server.StartGame();
        }

        public static void MiniGameOver(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            MiniGames finishedGame = (MiniGames)packet.ReadInt();
            var temp = Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint;
        
            Console.WriteLine($"{temp}: has completed minigame {finishedGame}");
            
            if (clientIdCheck != fromClient)
                Console.WriteLine($"{temp} has assumed the wrong client ID. monitor ID: {clientIdCheck}");
            
            Server.isPlaying = false;
            Server.StartGame(finishedGame);
        }
    }
}