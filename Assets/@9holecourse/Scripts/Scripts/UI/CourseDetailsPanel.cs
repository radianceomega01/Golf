using TMPro;
using UnityEngine;

public class CourseDetailsPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI courseName;
    [SerializeField] private TextMeshProUGUI coursePar;

    private void OnEnable()
    {
        if (CourseManager.Instance == null)
            return;

        Course activeCourse = CourseManager.Instance.ActiveCourse;
        courseName.text = "Course " + (GameManager.Instance.ActiveCourseIndex+1).ToString();
        coursePar.text = "Par "+ activeCourse.par + " Course";
    }
}
