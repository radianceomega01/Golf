
[System.Serializable]
public class StatsData
{
    public int luck;
    public int power;
    public int accuracy;
    public int drives;
    public int winds;
    public int putts;

    public void SetData(int luck, int power, int accuracy, int drives, int winds, int putts)
    {
        this.luck = luck;
        this.power = power;
        this.accuracy = accuracy;
        this.drives = drives;
        this.winds = winds;
        this.putts = putts;
    }

    public int GetLuck() => luck;
    public int GetPower() => power;
    public int GetAccuracy() => accuracy;
    public int GetDrives() => drives;
    public int GetWinds() => winds;
    public int GetPutts() => putts;
}
