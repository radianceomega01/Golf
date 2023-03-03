
public class ShotData
{
    string area;
    int yards;
    int penalty;

    public ShotData(string area, int yards)
    {
        this.area = area;
        this.yards = yards;

        if (area == "Bunkers" || area == "Water")
            penalty = 1;
        else
            penalty = 0;
    }
}
