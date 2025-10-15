using UnityEngine;

public class MiniGame : MonoBehaviour
{
    public MiniGames gameId { get; set; }
    public virtual void StartMiniGame(){}

    public void MiniGameFinished()
    {
        ClientSend.MiniGameOver(gameId);
    }
}