using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AtkCtrlJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    //스킬 조이스틱을 드래그 하면 공격 범위가 나오고 손을 떼면 기본공격이 나감
    //스킬은 PlayerPrefab 의 자식 오브젝트 Weapon.Attack이 호출됨
    //조이스틱이 다시 중앙에 위치하면 공격 취소

    private Image backGroundImg;
    private Image joystickImg;
    private Vector3 inputVector;
    //private Aim aim;

    void Start()
    {
        backGroundImg = GetComponent<Image>();
        joystickImg = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnDrag(PointerEventData _pointEventData)
    {
        //드래그 하는 방향대로 플레이어 캐릭터를 중심으로 공격범위가 나타남
        //gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true; ;
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
        //inputVector로 공격을 함
        //gameObject.transform.GetChild(0).localEulerAngles.Set(0f, 0f, 0f);
        //gameObject.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        //gameObject.transform.rotation = Vector3.zero;
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
