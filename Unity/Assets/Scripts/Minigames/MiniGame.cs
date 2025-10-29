using UnityEngine;
using UnityEngine.Serialization;

public class MiniGame : MonoBehaviour
{
    public MiniGames gameId { get; set; }
    [HideInInspector]
    public bool isGameRunning = false;

    public virtual void StartMiniGame(bool shouldShowControls)
    {
        isGameRunning = true;
    }

    public virtual void MiniGameFinished()
    {
        isGameRunning = false;
        ClientSend.MiniGameOver(gameId);
    }
}