using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillCtrlJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    //스킬 조이스틱을 드래그 하면 공격 범위가 나오고 손을 떼면 스킬이 나감
    //스킬은 PlayerPrefab 의 자식 오브젝트 Weapon.UseSkill이 호출됨
    //조이스틱이 다시 중앙에 위치하면 스킬 사용을 취소
    private Controller controller;

    private Image backGroundImg;
    private Image joystickImg;
    private Vector3 inputVector;

    public bool isUsing;

    protected void Start()
    {
        backGroundImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
        controller = GameObject.Find("Controller").GetComponent<Controller>();
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
        isUsing = true;
        OnDrag(_pointEventData);
    }

    public virtual void OnPointerUp(PointerEventData _pointEventData)
    {
        isUsing = false;
        if (inputVector != Vector3.zero)
        {
            //controller.targetObj.GetComponent<Player>().UseSkill(inputVector);
        }
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
