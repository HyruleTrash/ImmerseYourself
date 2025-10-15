using System;
using UnityEngine;

public class AreYouSmartEnoughGame : MiniGame
{
    private void Start()
    {
        gameId = MiniGames.AreYouSmartEnough;
    }

    public override void StartMiniGame()
    {
        Debug.Log("Are you smart enough has been started..");
    }
}