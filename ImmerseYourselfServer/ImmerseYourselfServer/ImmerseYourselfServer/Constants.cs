namespace ImmerseYourselfServer;

public class Constants
{
    public const int TICKS_PER_SECOND = 30;
    public const int MS_PER_TICK = 1000 / TICKS_PER_SECOND;
    public const string DISCORD_API_TOKEN = "";
}

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

public static class EnumExtensions
{
    public static T GetRandomEnumValue<T>() where T : Enum
    {
        var values = (T[])Enum.GetValues(typeof(T));
        return values[Random.Shared.Next(values.Length)];
    }
}

public static class DictionaryExtensions
{
    private static readonly Random _random = new Random();
    
    public static KeyValuePair<TKey, TValue>? GetRandomEntry<TKey, TValue>(
        this Dictionary<TKey, TValue> dict)
    {
        if (dict.Count == 0)
            return null;
            
        var keys = new List<TKey>(dict.Keys);
        TKey randomKey = keys[_random.Next(keys.Count)];
        return new KeyValuePair<TKey, TValue>(randomKey, dict[randomKey]);
    }
}