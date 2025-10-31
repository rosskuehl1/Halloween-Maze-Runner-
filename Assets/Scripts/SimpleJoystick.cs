using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image bg;
    public Image knob;
    public float radius = 80f;

    private Vector2 value;
    public Vector2 Value => value;

    public void OnPointerDown(PointerEventData e) => OnDrag(e);

    public void OnDrag(PointerEventData e)
    {
        RectTransform rt = (RectTransform)bg.transform;
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, e.position, e.pressEventCamera, out local);
        local = Vector2.ClampMagnitude(local, radius);
        knob.rectTransform.anchoredPosition = local;
        value = local / radius;
    }

    public void OnPointerUp(PointerEventData e)
    {
        knob.rectTransform.anchoredPosition = Vector2.zero;
        value = Vector2.zero;
    }
}
