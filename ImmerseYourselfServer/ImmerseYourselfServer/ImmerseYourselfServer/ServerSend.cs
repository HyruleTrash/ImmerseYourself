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
    }
}

