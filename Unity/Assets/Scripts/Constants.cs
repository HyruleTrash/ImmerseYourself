
public enum MiniGames
{
    TerMaanTerOceaanInDeRuimtVerVandaan = 1,
    AlienLingo = 2,
    AreYouSmartEnough = 3,
}

/// <summary>Sent from server to client.</summary>
public enum ServerPackets
{
    Welcome = 1,
    ServerFull = 2,
    StartMiniGame = 3,
    ShowWaiting = 4
}

/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    WelcomeReceived = 1,
    MiniGameOver = 2
}