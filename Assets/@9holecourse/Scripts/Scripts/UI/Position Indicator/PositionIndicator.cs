using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PositionIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform OnScreenIndicator;
    [SerializeField] private RectTransform OffScreenIndicator;
    [SerializeField] private Sprite arrowHead;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private IndicatorType golferType;
    [SerializeField] private TextMeshProUGUI golferTextOffScreen;
    [SerializeField] private TextMeshProUGUI golferTextOnScreen;
    [SerializeField] private bool clamp;

    private Transform target;
    private RectTransform parentRectTransform;
    private Image icon;

    float minX, maxX;
    float minY, maxY;
    Vector3 pos;
    bool isNotOnScreen;
    Vector2 initialSize;
    public Transform Target
    {
        set { target = value; }
    }

    public string Name { get; set; }

    private void Start()
    {
        parentRectTransform = gameObject.GetComponent<RectTransform>();
        initialSize = OffScreenIndicator.rect.size;
        icon = OffScreenIndicator.GetComponentInChildren<Image>();

        if (golferType != IndicatorType.Hole)
        {
            golferTextOffScreen.text = Name;
            golferTextOnScreen.text = Name;
        }
    }

    void Update()
    {
        if (target != null)
        {
            OffScreenIndicator.gameObject.SetActive(true);

            ScaleWithResolution();

            minX = OffScreenIndicator.rect.width / 2;
            maxX = Screen.width - minX;

            minY = OffScreenIndicator.rect.height / 2;
            maxY = Screen.height - minY;

            pos = Camera.main.WorldToScreenPoint(target.position);

            if (clamp)
                isNotOnScreen = (pos.x > Screen.width) || (pos.x < 0) || (pos.y > Screen.height) || (pos.y < 0);
            else
                isNotOnScreen = false;


            if (isNotOnScreen)
                ChangeIndicatorAndRotation();
            else
            {
                OnScreenIndicator.gameObject.SetActive(true);
                OffScreenIndicator.gameObject.SetActive(false);
                OnScreenIndicator.transform.position = pos;
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            pos.z = 0;

            OffScreenIndicator.transform.position = pos;
            
        }
        else
        {
            OffScreenIndicator.gameObject.SetActive(false);
            OnScreenIndicator.gameObject.SetActive(false);
        }
    }

    private void ChangeIndicatorAndRotation()
    {
        OnScreenIndicator.gameObject.SetActive(false);
        OffScreenIndicator.gameObject.SetActive(true);

        float delX = pos.x - icon.transform.position.x;
        float delY = pos.y - icon.transform.position.y;
        //float delX = transform.position.x - icon.transform.position.x;
        //float delY = transform.position.y - icon.transform.position.y;
        float angle = Mathf.Atan2(delY, delX) * Mathf.Rad2Deg;
        icon.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void ScaleWithResolution()
    {
        float multiplier = (float)Screen.height / 1080;
        parentRectTransform.sizeDelta = initialSize * multiplier;
    }

    private enum IndicatorType
    { 
        Player,
        Opponent,
        Hole
    }
}
