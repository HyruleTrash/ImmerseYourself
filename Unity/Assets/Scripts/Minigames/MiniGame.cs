using UnityEngine;
using UnityEngine.Serialization;

public class MiniGame : MonoBehaviour
{
    public MiniGames gameId { get; set; }
    public bool isGameRunning = false;

    public virtual void StartMiniGame()
    {
        isGameRunning = true;
    }

    public virtual void MiniGameFinished()
    {
        isGameRunning = false;
        ClientSend.MiniGameOver(gameId);
    }
}