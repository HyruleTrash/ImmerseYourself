
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

    #endregion
}