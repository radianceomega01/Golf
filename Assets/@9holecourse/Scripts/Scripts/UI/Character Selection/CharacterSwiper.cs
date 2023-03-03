using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSwiper : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] SwipeDirection direction;

    private Button button;
    private float swipeSpeed = 0.1f;
    private void Awake()
    {
        button = gameObject.GetComponent<Button>();
        if (direction == SwipeDirection.Left)
            button.onClick.AddListener(OnLeftSwipeClicked);
        else
            button.onClick.AddListener(OnRightSwipeClicked);

    }

    public void OnLeftSwipeClicked()
    {
        if (scrollRect.horizontalNormalizedPosition >= 0)
            scrollRect.horizontalNormalizedPosition -= swipeSpeed;
    }

    public void OnRightSwipeClicked()
    {
        if (scrollRect.horizontalNormalizedPosition <= 1)
            scrollRect.horizontalNormalizedPosition += swipeSpeed;
    }

    enum SwipeDirection
    { 
        Left,
        Right
    }
}
