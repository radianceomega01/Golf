using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardUsersUI : SingletonBehaviour<ScoreboardUsersUI>
{
    [SerializeField] GameObject scoreboardUser;

    public void Initialize()
    {
        int count = ScoreBoard.Instance.GetScoreCards().Count;

        //for header
        GameObject spawnedObj = Instantiate(scoreboardUser, transform);
        spawnedObj.GetComponent<TextMeshProUGUI>().text = "Player";

        //for players
        for (int i = 1; i <= count; i++)
        {
            spawnedObj = Instantiate(scoreboardUser, transform);
            spawnedObj.GetComponent<TextMeshProUGUI>().text = ScoreBoard.Instance.GetScoreCards()[i].name;
        }
    }
}
