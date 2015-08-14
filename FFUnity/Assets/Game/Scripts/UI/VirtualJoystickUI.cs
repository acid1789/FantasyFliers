using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class VirtualJoystickUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public bool useHorizontalAxis = false;
    public bool useVerticalAxis = false;

    public float horizontalMovementRange = 100f;
    public float verticalMovementRange = 100f;
    public float deadZone = 0.19f;

    private Vector2 deadCenter;
    private float horizontalAxis = 0f;
    private float verticalAxis = 0f;
    private RectTransform rectTrans;

    // Use this for initialization
    void Start()
    {
        EventSystem system = GameObject.FindObjectOfType<EventSystem>();

        if(system == null)
        {
            GameObject obj = new GameObject("EventSystem");

            obj.AddComponent<EventSystem>();
            obj.AddComponent<StandaloneInputModule>();
            obj.AddComponent<TouchInputModule>();
        }

        rectTrans = gameObject.GetComponent<RectTransform>();
        deadCenter = rectTrans.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAxes(Vector2 value)
    {
        Vector2 delta = deadCenter - value;
        delta.x /= horizontalMovementRange;
        delta.y /= verticalMovementRange;
        
        float updateX = Mathf.Clamp(delta.x, -1f, 1f);
        float updateY = Mathf.Clamp(delta.y, -1f, 1f);

        if (Mathf.Abs(updateX) > deadZone)
            horizontalAxis = updateX;
        else
            horizontalAxis = 0f;

        if (Mathf.Abs(updateY) > deadZone)
            verticalAxis = updateY;
        else
            verticalAxis = 0f; 
    }

    public void OnDrag(PointerEventData data)
    {
        float deltaX = 0;
        float deltaY = 0;

        if (useHorizontalAxis)
        {
            deltaX = (data.position.x - data.pressPosition.x) - deadCenter.x;
            deltaX = Mathf.Clamp(deltaX, -horizontalMovementRange, horizontalMovementRange); 
        }

        if (useVerticalAxis)
        {
            deltaY = (data.position.y - data.pressPosition.y) - deadCenter.y;
            deltaY = Mathf.Clamp(deltaY, -verticalMovementRange, verticalMovementRange); 
        }

        rectTrans.anchoredPosition = new Vector2(deadCenter.x + deltaX, deadCenter.y + deltaY);
        UpdateAxes(rectTrans.anchoredPosition);
    }

    public void OnPointerUp(PointerEventData data)
    {
        rectTrans.anchoredPosition = deadCenter;
        UpdateAxes(rectTrans.anchoredPosition);
    }

    public void OnPointerDown(PointerEventData data)
    {
        
    }

    public float HorizontalAxis { get { return horizontalAxis; } }

    public float VerticalAxis { get { return verticalAxis; } }
}
