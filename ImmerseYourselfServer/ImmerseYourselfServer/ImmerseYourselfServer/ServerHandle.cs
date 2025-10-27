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

            if (!Server.IsServerFull()) return;
            ServerSend.TriggerMinigameWaitAll();
            Server.discordBotManager.AnnounceMessageAsync("Notification", "Monitors are ready to start a minigame", "Enter command !next OR !start");
        }

        public static void MiniGameOver(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            MiniGames finishedGame = (MiniGames)packet.ReadInt();
            var temp = Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint;
        
            Console.WriteLine($"{temp}: has completed minigame {finishedGame}");
            Server.discordBotManager.AnnounceMessageAsync("Notification", "Player finished minigame:", finishedGame.ToString());
            
            if (clientIdCheck != fromClient)
                Console.WriteLine($"{temp} has assumed the wrong client ID. monitor ID: {clientIdCheck}");
            
            Server.gameData.isPlaying = false;
            Server.gameData.lastPlayedMiniGame = finishedGame;

            ServerSend.TriggerMinigameWait(Server.clients[fromClient].tcp);
        }
    }
}