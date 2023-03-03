using UnityEngine;

public class Course : MonoBehaviour
{
    public Transform teeing;
    public Transform hole;
    public Transform waterPoints;
    public CourseData data;
    public int par = 3;
    public int maxAllowedShots;

    private void Awake()
    {
        GetComponentInChildren<Cinematics>().gameObject.SetActive(false);
        GetComponentInChildren<OverviewCamera>().gameObject.SetActive(false);
        maxAllowedShots = par + 2;
    }
}
