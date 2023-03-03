using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CourseScoresUI : MonoBehaviour
{
    [SerializeField] private GameObject courseScore;

    public void Initialize(int courseIndex)
    {
        int count = ScoreBoard.Instance.GetScoreCards().Count;

        //for Header
        GameObject spawnedObj = Instantiate(courseScore, transform);
        spawnedObj.GetComponent<TextMeshProUGUI>().text = "Course " + (courseIndex + 1).ToString();

        // for Scores
        for (int i = 1; i <= count; i++)
        {
            spawnedObj = Instantiate(courseScore, transform);
            spawnedObj.GetComponent<TextMeshProUGUI>().text = ScoreBoard.Instance.GetGolferScore(i)[courseIndex].ToString();
        }
    }
}
