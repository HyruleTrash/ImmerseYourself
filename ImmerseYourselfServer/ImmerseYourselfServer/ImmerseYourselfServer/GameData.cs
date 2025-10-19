using ImmerseYourselfServer;

public class GameData
{
    public bool isPlaying = false;
    private List<Tuple<int, MiniGames>> playedMiniGames = [];
    
    public bool PlayedMiniGamesContains(MiniGames game)
    {
        return playedMiniGames.Any(item => game == item.Item2);
    }

    public int GetLastPickedClientForMiniGame(MiniGames game)
    {
        foreach (var item in playedMiniGames)
        {
            if (game == item.Item2)
                return item.Item1;
        }
        return -1;
    }

    /// <summary>
    /// If the game already remembers that the minigame has been played, it will just update which client last played it
    /// </summary>
    /// <param name="game"></param>
    /// <param name="clientId"></param>
    public void AddMiniGameToPlayedMiniGames(MiniGames game, int clientId)
    {
        bool foundGame = false;
        for (var index = 0; index < playedMiniGames.Count; index++)
        {
            if (game != playedMiniGames[index].Item2) continue;
            playedMiniGames[index] = new Tuple<int, MiniGames>(clientId, game);
            foundGame = true;
            break;
        }

        if (!foundGame)
            playedMiniGames.Add(new Tuple<int, MiniGames>(clientId, game));
    }
}