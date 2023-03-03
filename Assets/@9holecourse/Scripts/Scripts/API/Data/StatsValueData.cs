
[System.Serializable]
public class StatsValueData
{
    public float luck;
    public float inAccuracy;
    public float drives;
    public float inaccPutts;
    public float inaccWinds;

    public void SetData(float luck, float inAccuracy, float drives, float inaccPutts, float inaccWinds)
    {
        this.luck = luck;
        this.inAccuracy = inAccuracy;
        this.drives = drives;
        this.inaccPutts = inaccPutts;
        this.inaccWinds = inaccWinds;
    }

    public float GetLuck() => luck;
    public float GetInaccuracy() => inAccuracy;
    public float GetDrives() => drives;
    public float GetInaccPutts() => inaccPutts;
    public float GetInaccWinds() => inaccWinds;
}
