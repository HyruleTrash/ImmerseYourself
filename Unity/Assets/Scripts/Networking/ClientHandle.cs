using ImmerseYourselfServer;
using UnityEngine;
using UnityRawInput;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string message = packet.ReadString();
        int id = packet.ReadInt();
        
        Debug.Log($"Message from server: {message}");
        UIManager.instance.EnableGameUI();
        
        Client.instance.id = id;
        Client.instance.tcp.isConnected = true;
        // #if !UNITY_EDITOR
        // Client.instance.MoveWindowToMonitor(id);
        // #endif
        
        RawInput.Start();
        RawInput.WorkInBackground = true;
        RawInput.InterceptMessages = false;
        
        ClientSend.WelcomeReceived();
    }
    
    public static void ServerFull(Packet packet)
    {
        Debug.Log("Server Full notice received");
        Client.instance.Disconnect();
    }
    
    public static void TriggerWait(Packet packet)
    {
        Debug.Log("waiting...");
        UIManager.instance.EnableGameWaitingUI();
    }
    
    public static void StartMiniGame(Packet packet)
    {
        UIManager.instance.DisableGameWaitingUI();
        var miniGameId = (MiniGames)packet.ReadInt();
        var shouldShowControls = packet.ReadBool();
        Debug.Log($"Mini game requested, starting MiniGame {miniGameId}");

        bool found = false;
        foreach (var miniGameComponent in Client.instance.miniGameHolder.miniGames)
        {
            if (miniGameComponent.gameId != miniGameId)
                continue;
            miniGameComponent.StartMiniGame(shouldShowControls);
            found = true;
            GUI.FocusWindow(0);
            break;
        }
        if (!found)
            ClientSend.MiniGameOver(miniGameId);
    }
}