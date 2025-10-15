using ImmerseYourselfServer;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string message = packet.ReadString();
        int id = packet.ReadInt();
        
        Debug.Log($"Message from server: {message}");
        UIManager.instance.EnableGameUI();
        
        Client.instance.id = id;
        #if !UNITY_EDITOR
        Client.instance.MoveWindowToMonitor(id);
        #endif
        
        ClientSend.WelcomeReceived();
    }
    
    public static void ServerFull(Packet packet)
    {
        Debug.Log("Server Full notice received");
        Client.instance.Disconnect();
    }
    
    public static void StartMiniGame(Packet packet)
    {
        var miniGameId = (MiniGames)packet.ReadInt();
        Debug.Log($"Mini game requested, starting MiniGame {miniGameId}");
        
        // TODO: actually start the mini game
    }
}