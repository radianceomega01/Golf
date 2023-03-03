using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI shots;
    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI status;

    public void Initialize(ScoreCard scoreCardData)
    {
        name.text = scoreCardData.name;
        shots.text = scoreCardData.Shots.ToString();
        score.text = scoreCardData.Score.ToString();
        title.text = scoreCardData.ShotTitle.ToString();
        status.text = scoreCardData.MatchResult.ToString();
    }

}
