using Cinemachine;
using UnityEngine;

public class TargetCamera : MonoBehaviour
{
    private Transform target;
    private new CinemachineVirtualCamera camera;

    public Transform Target => target;
    public CinemachineVirtualCamera Camera => camera;

    private void Awake()
    {
        target = transform.Find("Target");
        camera = GetComponentInChildren<CinemachineVirtualCamera>();
    }
    public void SetTargetPosition(Vector3 position)
    {
        target.transform.position = position;
    }

    public void SetTargetRotaion(Quaternion rotation)
    {
        target.transform.rotation = rotation;
    }
}
