using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class MobileJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform ControllerArea;
    [SerializeField] RectTransform Handle;

    Vector2 directionVector;

    void Awake()
    {
        Handle.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        directionVector = Vector2.zero;
        Handle.anchoredPosition = Vector2.zero;
    }

    void CalculateDirection(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            ControllerArea, eventData.position, eventData.pressEventCamera, out pos))
        {
            float radiusX = ControllerArea.sizeDelta.x/2;
            float radiusY = ControllerArea.sizeDelta.y/2;

            // Set Direction
            Vector2 direction = new Vector2(pos.x / radiusX, pos.y / radiusY);

            // Clamp the vector magnitude to 1 (circle)
            if (direction.magnitude > 1f)
            {
                direction.Normalize();
            }

            directionVector = direction;

            // Move Handle
            Handle.anchoredPosition = new Vector2(
                direction.x * radiusX,
                direction.y * radiusY
            );

            // Rotate handle
            if (directionVector != Vector2.zero)
            {
                float angle = Mathf.Atan2(-directionVector.x, directionVector.y) * Mathf.Rad2Deg;
                Handle.localRotation = Quaternion.Euler(0, 0, angle);
            }
            else Handle.localRotation = Quaternion.identity;
        }
    }

    #region ------------------ EVENTS------------------
    public void OnDrag(PointerEventData eventData)
    {
        CalculateDirection(eventData);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Handle.gameObject.SetActive(true);
        CalculateDirection(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        directionVector = Vector2.zero;
        Handle.anchoredPosition = Vector2.zero;
        Handle.localRotation = Quaternion.identity;
        Handle.gameObject.SetActive(false);
    }

    #endregion

    #region ------------------ GETTERS------------------
    public Vector3 GetDirection()
    {
        return new Vector3(Horizontal(), 0, Vertical());
    }
    public float Horizontal() => directionVector.x;
    public float Vertical() => directionVector.y;
    #endregion


}
