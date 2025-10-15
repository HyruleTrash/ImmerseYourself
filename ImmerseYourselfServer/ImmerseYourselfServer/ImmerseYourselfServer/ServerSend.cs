namespace ImmerseYourselfServer
{
    public class ServerSend
    {
        private static void SendTCPData(int clientId, Packet packet)
        {
            packet.WriteLength();
            Server.clients[clientId].tcp.SendData(packet);
        }
        
        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            foreach (var item in Server.clients)
            {
                item.Value.tcp.SendData(packet);
            }
        }
        
        public static void Welcome(int clientId, string message)
        {
            using var packet = new Packet((int)ServerPackets.Welcome);
            //  putting this in using will automatically dispose it when we don't use it no more (clean af)
            
            packet.Write(message);
            packet.Write(clientId);
                
            SendTCPData(clientId, packet);
        }
        
        public static void ServerFull(Client.TCP tcpClient)
        {
            using var packet = new Packet((int)ServerPackets.ServerFull);
            packet.WriteLength();
            tcpClient.SendData(packet);
        }

        public static void StartMiniGame(int clientId, MiniGames lastPlayedMiniGame = 0)
        {
            Console.WriteLine($"Starting MiniGame at monitor {clientId}");
            using var packet = new Packet((int)ServerPackets.StartMiniGame);

            MiniGames foundGame = GetRandomEnumValue<MiniGames>();
            while (foundGame == lastPlayedMiniGame)
            {
                foundGame = GetRandomEnumValue<MiniGames>();
                if (foundGame != lastPlayedMiniGame)
                    break;
            }
            packet.Write((int)foundGame);
            
            SendTCPData(clientId, packet);
        }
        
        public static T GetRandomEnumValue<T>() where T : Enum
        {
            var values = (T[])Enum.GetValues(typeof(T));
            return values[Random.Shared.Next(values.Length)];
        }
    }
}

