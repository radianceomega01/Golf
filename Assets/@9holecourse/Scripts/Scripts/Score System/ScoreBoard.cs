using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard
{
    Dictionary<int, ScoreCard> scoreCards = new Dictionary<int, ScoreCard>();

    List<List<int>> golferScores = new List<List<int>>();

    private static ScoreBoard instance = null;
    public static ScoreBoard Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ScoreBoard();
            }
            return instance;
        }
    }

    private ScoreBoard() { }

    public void AddScoreCard(int id, ScoreCard scorecard) => scoreCards.Add(id, scorecard);
    public void RemoveScoreCard(int id) => scoreCards.Remove(id);

    public void CreateGolferScore()
    {
        List<int> scores = new List<int>();
        golferScores.Add(scores);
    }
    public void AddGolferScore(int id, int score) => golferScores[id-1].Add(score); //id-1 because golferId starts from 1 and indexing from 0.
    public List<int> GetGolferScore(int id) => golferScores[id-1];//id-1 because golferId starts from 1 and indexing from 0.
    public void RemoveGolferScore(int id) => golferScores.Remove(golferScores[id-1]);

    public Dictionary<int, ScoreCard> GetScoreCards() => scoreCards;

    public void Clear() => scoreCards.Clear();
}
