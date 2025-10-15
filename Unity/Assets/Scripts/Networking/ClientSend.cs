
using ImmerseYourselfServer;
using UnityEngine;

class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    #region Packets

    public static void WelcomeReceived()
    {
        using var packet = new Packet((int)ClientPackets.WelcomeReceived);
        packet.Write(Client.instance.id);
        
        SendTCPData(packet);
    }
    
    public static void MiniGameOver(MiniGames gameId)
    {
        using var packet = new Packet((int)ClientPackets.MiniGameOver);
        packet.Write(Client.instance.id);
        packet.Write((int)gameId);
        
        SendTCPData(packet);
    }

    #endregion
}