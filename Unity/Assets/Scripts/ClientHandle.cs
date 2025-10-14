using ImmerseYourselfServer;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string message = packet.ReadString();
        int id = packet.ReadInt();
        
        Debug.Log($"Message from server: {message}");

        Client.instance.id = id;
        
        // TODO send message received
    }
}