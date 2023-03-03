using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubInput : MonoBehaviour
{
    public event Action<InputAxis> OnDragReleased;

    [SerializeField] private FixedJoystick fixedJoyStick;
    [SerializeField] private Needle needle;

    [HideInInspector] public bool isDraging;

    private InputAxis inputAxis;
    

    // Update is called once per frame
    void Update()
    {
        isDraging = fixedJoyStick.isDraging;
        if (isDraging)
        {
            inputAxis.force = Mathf.Abs(fixedJoyStick.input.y);
            inputAxis.angle = -fixedJoyStick.input.x;
        }

        if (fixedJoyStick.isDragReleased)
        {
            inputAxis.force = Mathf.Abs(fixedJoyStick.input.y);
            if (needle != null)
            {
                inputAxis.angle = -fixedJoyStick.input.x - needle.transform.localRotation.z;
                needle.ResetNeedle();
            }
            else
                inputAxis.angle = -fixedJoyStick.input.x;

            OnDragReleased?.Invoke(inputAxis);
            fixedJoyStick.isDragReleased = false;
        }
    }

    public InputAxis GetDragInputAxis() => inputAxis;

    public FixedJoystick GetFixedJoystick() => fixedJoyStick;

    public void Reset()
    {
        fixedJoyStick.isDraging = false;
        inputAxis.force = 0;
        inputAxis.angle = 0;
        if (needle != null)
        {
            needle.canRotate = false;
            needle.ResetNeedle();
        }
        fixedJoyStick.CustomOnPointerUp();
    }
}
