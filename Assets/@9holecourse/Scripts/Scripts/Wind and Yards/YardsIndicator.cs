using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YardsIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI yardsCount;

    private Ball ball;
    private float yardsConverter;

    private void Awake()
    {
        yardsConverter = 1.1f;
    }

    private void OnEnable()
    {
        if (MatchManager.Instance.GetActiveGolfer() != null)
            ball = MatchManager.Instance.GetActiveGolfer().Ball;
    }
    void Update()
    {
        if (ball == null)
            return;
        yardsCount.text = ((int)((CourseManager.Instance.ActiveCourse.hole.position - ball.transform.position).magnitude * yardsConverter)).ToString();
    }
}
