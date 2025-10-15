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
            
            // TODO: enter game
        }
    }
}