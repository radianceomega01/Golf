using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIPointerDragInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private int id;
    private Vector2 axis;
    private Vector2 previousPosition;
    private bool active;

    public Vector2 Axis => axis; 
    public int Id => id;

    private void Update()
    {
        if (!active)
        {
            axis = Vector2.zero;
            return;
        }

        if (id >= 0 && id < Input.touches.Length)
        {
            axis = Input.touches[id].position - previousPosition;
            previousPosition = Input.touches[id].position;
        }
        else
        {
            axis = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - previousPosition;
            previousPosition = Input.mousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        active = true;
        id = eventData.pointerId;
        previousPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        active = false;
    }
}
