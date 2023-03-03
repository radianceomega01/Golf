using UnityEngine;
using UnityEngine.SceneManagement;

public class CourseManager : SingletonBehaviour<CourseManager>
{
    [Header("Temparory")]
    public Course prefab;

    private Course activeCourse;
    public Course ActiveCourse => activeCourse;

    protected override void Awake()
    {
        base.Awake();
        SetCourse();
    }

    public void SetCourse()
    {
        prefab = CourseList.Instance.GetCourse(GameManager.Instance.ActiveCourseIndex);
        activeCourse = Instantiate(prefab, Vector3.up * 10, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(activeCourse.gameObject, SceneManager.GetSceneByName("Course"));
    }
}
