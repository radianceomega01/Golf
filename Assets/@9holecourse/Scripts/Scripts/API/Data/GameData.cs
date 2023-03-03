[System.Serializable]
public class GameData
{
    public int coins;
    public StatsData[] statsData = new StatsData[10];
    public StatsValueData[] statsValueData = new StatsValueData[10];

    static GameData instance = null;

    public static GameData Instance
    {
        get
        {
            if (instance == null)
                instance = new GameData();

            return instance;
        }
    }

    public void SetCoins(int coins) => this.coins = coins;

    public void AddStatsData(int index, StatsData data) => statsData[index] = data;
    public void AddStatsValueData(int index, StatsValueData data) => statsValueData[index] = data;

    public int GetCoins() => coins;
    public StatsData GetStatsData(int index) => statsData[index];
    public StatsValueData GetStatsValueData(int index) => statsValueData[index];
}
