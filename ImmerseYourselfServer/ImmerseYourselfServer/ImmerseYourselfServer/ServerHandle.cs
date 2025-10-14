namespace ImmerseYourselfServer
{
    public class ServerHandle
    {
        public static void WelcomeRecieved(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();
        
            Console.WriteLine($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint}: connected successfully! player name: {username}");
            if (clientIdCheck != fromClient)
                Console.WriteLine($"{username} has assumed the wrong client ID. Player ID: {clientIdCheck}");
            
            // TODO: enter game
        }
    }
}