using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needle: MonoBehaviour
{
    public float smoothness;
    public bool canRotate;
    bool rotateLeft;
    float delAngle;
    // Update is called once per frame
    void Update()
    {
        if (canRotate)
            RotateNeedle();
    }

    private void RotateNeedle()
    {
        // Never ever read and write values from Quaternion. They are not meant for human interaction!
        /*if (transform.localRotation.z >= 0.6f || transform.localRotation.z <= -0.6f)
            rotateLeft = !rotateLeft;*/

        delAngle = transform.eulerAngles.z;
        if (delAngle > 180)
            delAngle -= 360;

        if (Mathf.Abs(delAngle) > 75f)
            rotateLeft = !rotateLeft;

        if (rotateLeft)
            transform.Rotate(new Vector3(0f, 0f, 5f) * smoothness * Time.deltaTime);
        else
            transform.Rotate(new Vector3(0f, 0f, -5f) * smoothness * Time.deltaTime);
    }

    public void ResetNeedle()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}
