using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedJoystick : Joystick
{
    //[SerializeField] private Image img;
    [SerializeField] private Slider slider;
    //[SerializeField] private Gradient dragGradient;
    [SerializeField] private Needle needle;

    [HideInInspector] public bool isDragReleased;
    [HideInInspector] public bool isDraging;

    protected override void Start()
    {
        base.Start();
    }

    public void Update()
    {
        if (isDraging)
            UpdateDragPower();
    }

    private void UpdateDragPower()
    {
        needle.smoothness = 30f * input.y;
        //color = img.color;
        if (input.y < -0.3f)
        {
            needle.canRotate = true;
            //color.a = 1f;
        }
        else
        {
            needle.canRotate = false;
            //color.a = 0f;
        }
        //img.color = dragGradient.Evaluate(Mathf.Abs(input.y));
        slider.value = Mathf.Abs(input.y);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        isDraging = true;
        base.OnPointerDown(eventData);
    }

    public void CustomOnPointerUp()
    {
        base.OnPointerUp(null);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        isDragReleased = true;
        isDraging = false;
        //img.color = dragGradient.Evaluate(0f);
        slider.value = 0f;
        needle.canRotate = false;
        base.OnPointerUp(eventData);
    }
}