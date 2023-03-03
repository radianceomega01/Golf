
using System.Collections.Generic;

public class ScoreCard
{

    List<ShotData> shotDatas = new List<ShotData>();

    public string name { get; set; }
    public bool CourseComplete { get; private set; }
    
    public int Shots => shotDatas.Count;

    public int Id { get; set; }

    public ShotTitles ShotTitle
    {
        get;
        private set;
    }

    public int Score
    {
        get => CourseComplete ? (CourseManager.Instance.ActiveCourse.maxAllowedShots+1) - Shots : 0;
    }

    public MatchResult MatchResult
    {
        get => CourseComplete && Shots == MatchManager.Instance.BestCompletionShots ? MatchResult.Won : MatchResult.Lost;
    }

    public void AddShotData(ShotData shotData, bool courseComplete)
    {
        shotDatas.Add(shotData);
        CourseComplete = courseComplete;

        if (courseComplete)
            EvaluateShotTitle();
        else
            ShotTitle = ShotTitles.None;

    }

    public void EvaluateShotTitle()
    {
        if (shotDatas.Count == CourseManager.Instance.ActiveCourse.par)
            ShotTitle = ShotTitles.Par;
        else if (shotDatas.Count == CourseManager.Instance.ActiveCourse.par - 1)
            ShotTitle = ShotTitles.Birdie;
        else if (shotDatas.Count == CourseManager.Instance.ActiveCourse.par - 2)
            ShotTitle = ShotTitles.Eagle;
        else if (shotDatas.Count == CourseManager.Instance.ActiveCourse.par + 1)
            ShotTitle = ShotTitles.Boogie;
        else if (shotDatas.Count == CourseManager.Instance.ActiveCourse.par + 2)
            ShotTitle = ShotTitles.DoubleBoogie;
        else if (shotDatas.Count == CourseManager.Instance.ActiveCourse.par + 3)
            ShotTitle = ShotTitles.TripleBoogie;
    }

    public void Clear()
    {
        shotDatas.Clear();
    }

}

public enum ShotTitles
{ 
    Eagle,
    Birdie,
    Par,
    Boogie,
    DoubleBoogie,
    TripleBoogie,
    None
}
