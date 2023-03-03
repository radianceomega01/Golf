using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardScoresUI : SingletonBehaviour<ScoreboardScoresUI>
{
    [SerializeField] private GameObject courseScores;
    [SerializeField] private Transform content;

    public void Initialize()
    {
        for(int i=0; i< CourseList.Instance.GetCourseListSize(); i++)
        {
            GameObject spawnedObj = Instantiate(courseScores, content);
            spawnedObj.GetComponent<CourseScoresUI>().Initialize(i);
        }
    }
}
