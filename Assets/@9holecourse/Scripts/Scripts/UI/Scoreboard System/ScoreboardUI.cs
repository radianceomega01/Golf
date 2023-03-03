using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUI : MonoBehaviour
{
    [SerializeField] private ScoreCardUI scoreCardPrefab;
    [SerializeField] private GameObject scoreboardObject;

    private Dictionary<int, ScoreCard> scoreCards;

    public void SetScoreboardData()
    {
        scoreCards = ScoreBoard.Instance.GetScoreCards();
        int dataCount = scoreCards.Count;
        for (int i = 1; i <= dataCount; i++)
        {
            ScoreBoard.Instance.AddGolferScore(i, scoreCards[i].Score);
            Instantiate(scoreCardPrefab, scoreboardObject.transform).Initialize(scoreCards[i]);
        }
    }
}
