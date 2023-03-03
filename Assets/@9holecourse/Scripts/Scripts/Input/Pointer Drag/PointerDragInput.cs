using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerDragInput : MonoBehaviour
{
    private Vector2 axis;
    private Vector2 previousPosition;
    private bool active;

    public Vector2 Axis => axis;
    /// <summary>
    /// If the pointer is blocked by an UI graphic.
    /// </summary>
    public bool Blocked
    {
        get
        {
            if (EventSystem.current == false)
            {
                return false;
            }

            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            foreach (var raycastResult in raysastResults)
            {
                Graphic graphic;
                if (raycastResult.gameObject.TryGetComponent(out graphic))
                {
                    if (graphic.raycastTarget == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Blocked)
        {
            active = true;
            previousPosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            active = false;
        }

        if (!active)
        {
            axis = Vector2.zero;
            return;
        }

        axis = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - previousPosition;
        previousPosition = Input.mousePosition;
    }
}
