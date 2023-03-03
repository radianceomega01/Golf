using System;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PointerDragInput))]
public class OverviewCamera : MonoBehaviour
{
    [SerializeField] private Vector2 sensitivity;

    private Transform target;
    private PointerDragInput dragInput;
    private new CinemachineVirtualCamera camera;
    private Collider confiner;
    private bool live;

    public event Action OnLive;

    private void Awake()
    {
        target = transform.Find("Target");
        target.parent = transform;
        dragInput = GetComponent<PointerDragInput>();
        camera = GetComponentInChildren<CinemachineVirtualCamera>();
        confiner = GetComponentInChildren<Collider>();
    }

    private void Update()
    {
        UpdateCameraStatus();
        UpdateMovement();
    }

    private void OnDisable() => live = false;

    public void SetTargetPosition(Vector3 position) => target.transform.position = position;

    public void SetTargetRotaion(Quaternion rotation) => target.transform.rotation = rotation;

    private void UpdateCameraStatus()
    {
        if (live)
            return;

        var brain = CinemachineCore.Instance.FindPotentialTargetBrain(camera);
        if (brain == null || brain.IsBlending)
            return;

        if (brain.IsLive(camera))
        {
            live = true;
            OnLive.Invoke();
        }
    }

    private void UpdateMovement()
    {
        if (dragInput.Blocked)
            return;

        var axis = -dragInput.Axis;
        var forward = camera.transform.forward;
        forward.y = 0;
        var translation = forward * axis.y * sensitivity.y * Time.deltaTime;
        target.Translate(translation, Space.World);
        var right = camera.transform.right;
        right.y = 0;
        translation = right * axis.x * sensitivity.x * Time.deltaTime;
        target.Translate(translation, Space.World);

        ClampTargetPosition();
    }

    private void ClampTargetPosition()
    {
        var position = target.transform.position;
        var bounds = confiner.bounds;

        position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
        position.y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
        position.z = Mathf.Clamp(position.z, bounds.min.z, bounds.max.z);

        target.transform.position = position;
    }
}
