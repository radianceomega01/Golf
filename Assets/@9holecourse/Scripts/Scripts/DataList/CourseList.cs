
using Blueberry.Core.AddressableAssets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "CourseList", menuName = "Scriptable Objects/CourseList", order = 6)]
public class CourseList : ScriptableObject
{
    [SerializeField] private Course[] courses;

    public static CourseList Instance { get; private set; }

    public static async Task Initialize()
    {
        Instance = await AddressablesManager.LoadAsset<CourseList>("CourseList");
    }

    public Course GetCourse(int index) => courses[index];

    public int GetCourseListSize() => courses.Length;
}
