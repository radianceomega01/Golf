using UnityEngine;
using UnityEngine.UI;

public class ShotAngleController : MonoBehaviour
{
    [SerializeField] private float limitInDegrees = 180f;
    [SerializeField] private float sensitivity = 6;

    //private UIPointerDragInput dragInput;
    private Slider slider;
    private Golfer golfer;
    private Vector3 forward;
    private RaycastHit hit;
    private Vector3 currentRotation;
    public bool Valid => hit.collider.gameObject.layer == 3;

    private void Awake()
    {
        //dragInput = GetComponent<UIPointerDragInput>();
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(UpdateRotation);
    }

    private void OnEnable()
    {
        golfer = MatchManager.Instance.GetActiveGolfer();
        forward = golfer.transform.forward;
        currentRotation = golfer.transform.localEulerAngles;
        slider.value = 0f;
        DrawTrajectory();
    }

    private void Update()
    {
        DrawTrajectory();
    }
    private void UpdateRotation(float angle)
    {
        golfer.transform.localEulerAngles = Vector3.up * angle + currentRotation;

        /*golfer.transform.Rotate(Vector2.up * (-dragInput.Axis.x * sensitivity) * Time.deltaTime);
        var angle = Vector3.SignedAngle(forward, golfer.transform.forward, Vector3.up);
        float exceededAngle = limitInDegrees - Mathf.Abs(angle);
        if (exceededAngle > 0)
            return;

        if (angle < 0)
        {
            golfer.transform.Rotate(Vector3.up, -exceededAngle);
        }
        else if (angle > 0)
        {
            golfer.transform.Rotate(Vector3.up, exceededAngle);
        }*/
    }

    private void DrawTrajectory()
    {
        var force = golfer.GetForce(InputAxis.Default());
        int layerMask = LayerMask.GetMask(LayerMask.LayerToName(3));
        Trajectory.Instance.Draw(golfer.transform.position, force, out hit, layerMask);
    }
}
