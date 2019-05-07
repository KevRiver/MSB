using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image backGroundImg;
    private Image joystickImg;
    private Vector3 inputVector;

    void Start()
    {
        backGroundImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnDrag(PointerEventData _pointEventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backGroundImg.rectTransform, _pointEventData.position, _pointEventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / backGroundImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / backGroundImg.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2, pos.y * 2, 0);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            joystickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (backGroundImg.rectTransform.sizeDelta.x / 3), inputVector.y * (backGroundImg.rectTransform.sizeDelta.y / 3));
        }
    }

    public virtual void OnPointerDown(PointerEventData _pointEventData)
    {
        OnDrag(_pointEventData);
    }

    public virtual void OnPointerUp(PointerEventData _pointEventData)
    {
        inputVector = Vector3.zero;
        joystickImg.rectTransform.anchoredPosition = Vector3.zero;
    }

    public float GetHorizontalValue()
    {
        return inputVector.x;
    }

    public float GetVerticalValue()
    {
        return inputVector.y;
    }
}
