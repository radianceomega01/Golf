using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory : SingletonBehaviour<Trajectory>
{
    [SerializeField] private new LineRenderer renderer;
    [Min(3)] [SerializeField] private int segments = 3;

    private List<Vector3> positions = new List<Vector3>();

    PhotonView photonView;

    protected override void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void Draw(Vector3 origin, Vector3 force, out RaycastHit hit, LayerMask layerMask)
    {
        renderer.positionCount = 0;
        CreateRaycast(origin, force, out hit, layerMask);

        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
            photonView.RPC("DrawForOthers", RpcTarget.Others, origin, force, (int)layerMask);

        renderer.positionCount = positions.Count;
        renderer.SetPositions(positions.ToArray());
    }

    [PunRPC]
    private void DrawForOthers(Vector3 origin, Vector3 force, int layerMask)
    {
        RaycastHit hit;

        renderer.positionCount = 0;
        CreateRaycast(origin, force, out hit, layerMask);
        renderer.positionCount = positions.Count;
        renderer.SetPositions(positions.ToArray());
    }

    public void CreateRaycast(Vector3 origin, Vector3 force, out RaycastHit hit, LayerMask layerMask)
    {
        positions.Clear();
        hit = new RaycastHit();
        var elevation = origin.y;
        var yaw = Yaw(force);
        force = -force.RotateAroundPointByAngle(Vector3.zero, Vector3.up * -yaw);
        var velocity = new Vector2(force.z, force.y);
        var delta = Range(velocity, elevation) / segments;
        for (int i = 0; i <= segments; i++)
        {
            var x = delta * i;
            var y = YPosition(velocity, elevation, x);
            var position = new Vector3(0, y, x);
            position = -position.RotateAroundPointByAngle(Vector3.zero, Vector3.up * yaw);
            position.x += origin.x;
            position.z += origin.z;
            positions.Add(position);
            if (i == 0)
                continue;

            var previousPosition = positions[i - 1];
            var direction = position - previousPosition;
            if (Physics.Raycast(previousPosition, direction.normalized, out RaycastHit raycastHit, direction.magnitude, layerMask))
            {
                hit = raycastHit;
                positions[i] = raycastHit.point;
                break;
            }
        }
    }

    public void Clear()
    {
        positions.Clear();
        renderer.positionCount = 0;

        if (GameManager.Instance.GameMode != GameManager.GameModes.SinglePlayer)
            photonView.RPC("ClearForOthers", RpcTarget.Others);
    }

    [PunRPC]
    public void ClearForOthers()
    {
        positions.Clear();
        renderer.positionCount = 0;
    }

    public void SetWidthMultiplier(float widthMultiplier)
    {
        renderer.widthMultiplier = widthMultiplier;
    }

    public void SetWidthCurve(AnimationCurve widthCurve)
    {
        renderer.widthCurve = widthCurve;
    }

    public void SetColor(Color color)
    {
        var gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(color, 0),
            new GradientColorKey(color, 1)
        };

        SetGradient(gradient);
    }

    public void SetGradient(Gradient gradient)
    {
        renderer.colorGradient = gradient;
    }

    private static float Yaw(Vector3 vector)
    {
        var forceDirection = (vector - Vector3.zero).normalized;
        var polarAxisDirection = (new Vector3(0, 1, 1) - Vector3.zero).normalized;
        var forceDirection2D = new Vector2(forceDirection.x, forceDirection.z);
        var polarAxisDirection2D = new Vector2(polarAxisDirection.x, polarAxisDirection.z);
        return Vector2.SignedAngle(forceDirection2D, polarAxisDirection2D);
    }

    private static float Range(Vector2 velocity, float elevation)
    {
        // d = V₀ * cos(α) * [V₀ * sin(α) + √((V₀ * sin(α))² + 2 * g * h)] / g
        float gravity = -Physics.gravity.y;
        float angleInRadians = Vector2.Angle(Vector2.right * velocity.y, velocity) * Mathf.Deg2Rad;
        float vMagnitude = velocity.magnitude;
        float xComponent = vMagnitude * Mathf.Cos(angleInRadians);
        float yComponent = vMagnitude * Mathf.Sin(angleInRadians);
        return xComponent * (yComponent + Mathf.Sqrt((yComponent * yComponent) + 2 * gravity * elevation)) / gravity;
    }

    private static float TimeOfFlight(Vector2 velocity, float elevation)
    {
        // t = [V₀ * sin(α) + √((V₀ * sin(α))² + 2 * g * h)] / g
        float gravity = -Physics.gravity.y;
        float angleInRadians = Vector2.Angle(Vector2.right * velocity.y, velocity) * Mathf.Deg2Rad;
        float vMagnitude = velocity.magnitude;
        float yComponent = vMagnitude * Mathf.Sin(angleInRadians);
        return (yComponent + Mathf.Sqrt((yComponent * yComponent) + 2 * gravity * elevation)) / gravity;
    }

    private static float YPosition(Vector2 velocity, float elevation, float x)
    {
        // y = h + x * (V₀ * sin(α)) / (V₀ * cos(α)) - g * (x / V₀ * cos(α))² / 2
        float gravity = -Physics.gravity.y;
        float angleInRadians = Vector2.Angle(Vector2.right * velocity.y, velocity) * Mathf.Deg2Rad;
        float vMagnitude = velocity.magnitude;
        float xComponent = vMagnitude * Mathf.Cos(angleInRadians);
        float yComponent = vMagnitude * Mathf.Sin(angleInRadians);
        return elevation + x * (yComponent / xComponent) - gravity * ((x / xComponent) * (x / xComponent)) / 2;
    }
}
