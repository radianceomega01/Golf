using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindIndicator : MonoBehaviour
{
    [SerializeField] private Sprite none;
    [SerializeField] private Sprite forward;
    [SerializeField] private Sprite forwardLeft;
    [SerializeField] private Sprite forwardRight;
    [SerializeField] private Sprite backward;
    [SerializeField] private Sprite backwardLeft;
    [SerializeField] private Sprite backwardRight;
    [SerializeField] private Sprite left;
    [SerializeField] private Sprite right;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI forceValue;

    private Transform hole;
    private Transform golfer;
    private float rotationAngle;
    private WindManager windManager;
    private void Awake()
    {
        windManager = WindManager.Instance;
        windManager.OnWindChanged += UpdateInfo;
        hole = CourseManager.Instance.ActiveCourse.hole;
    }

    private void OnEnable()
    {
        UpdateInfo(windManager.Wind);
    }

    void UpdateInfo(Vector3 wind)
    {
        //rotationAngle = GolferToHoleAngle();
        SetIcon(wind);
        SetIconRotation(rotationAngle);
        SetMagnitute(wind.magnitude);
    }

    private void SetIconRotation(float angle)
    {
        icon.GetComponent<RectTransform>().Rotate(0,0,angle);
    }

    private void SetMagnitute(float magnitude)
    {
        forceValue.text = Math.Round(magnitude,2).ToString();
    }

    private float GolferToHoleAngle()
    {
        golfer = MatchManager.Instance.GetActiveGolfer().transform;
        Vector3 holeDir = hole.position - golfer.position;
        float angle = Vector3.Angle(holeDir, golfer.forward);
        return angle;
    }
    private void SetIcon(Vector3 direction)
    {
        if (direction.x == 0 && direction.z == 0) // no wind
            icon.sprite = none;
        else if (direction.x == 0f && direction.z < 0f)
            icon.sprite = backward;
        else if (direction.x < 0f && direction.z < 0f)
            icon.sprite = backwardLeft;
        else if (direction.x > 0f && direction.z < 0f)
            icon.sprite = backwardRight;
        else if (direction.x == 0f && direction.z > 0f)
            icon.sprite = forward;
        else if (direction.x < 0f && direction.z > 0f)
            icon.sprite = forwardLeft;
        else if (direction.x > 0f && direction.z > 0f)
            icon.sprite = forwardRight;
        else if (direction.x < 0f && direction.z == 0f)
            icon.sprite = left;
        else if (direction.x > 0f && direction.z == 0f)
            icon.sprite = right;
    }

    private void OnDisable()
    {
        //SetIconRotation(-rotationAngle);
    }
    private void OnDestroy()
    {
        windManager.OnWindChanged -= UpdateInfo;
    }
}
