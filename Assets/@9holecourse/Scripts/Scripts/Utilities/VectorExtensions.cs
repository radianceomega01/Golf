using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 RotateAroundPointByAngle(this Vector3 vector, Vector3 point, Vector3 angleInDegrees)
    {
        var direction = point - vector;
        direction = Quaternion.Euler(angleInDegrees) * direction;
        return direction + point;
    }
}
