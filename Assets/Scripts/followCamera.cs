using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCamera : MonoBehaviour
{
    public Transform target = null;
    public float dist = 10.0f;
    public float height = 5.0f;
    public float smoothRotate = 5.0f;
    public Transform tr;
    public Vector3 followPosition;

    // Map으로부터 받은 높이
    public int mapHeight;
    public int mapWidth;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        float currYAngle = Mathf.LerpAngle(tr.eulerAngles.y, target.eulerAngles.y, smoothRotate * Time.deltaTime);

        Quaternion rot = Quaternion.Euler(0, currYAngle, 0);

        followPosition = target.position - (rot * Vector3.forward * dist) + (Vector3.up * 0);

        // 카메라 한계 설정 화면 비율과 크기에 따라 변동 시켜야됨 
        float bottomHeightLimit = 5.5f - mapHeight;
        if(followPosition.y <= bottomHeightLimit)
        {
            followPosition.y = bottomHeightLimit;
        }else if (followPosition.y >= -4.5)
        {
            followPosition.y = -4.5f;
        }

        float rightWidthLimit = mapWidth - 9.7f;
        if (followPosition.x >= rightWidthLimit)
        {
            followPosition.x = rightWidthLimit;
        }else if (followPosition.x <= 8.7)
        {
            followPosition.x = 8.7f;
        }

        // 카메라가 플레이어의 위치에 따라 이동
        tr.position = followPosition;

        // 카메라가 플레이어 주시 - 카메라가 맵 밖으로 나가지 않는 기능때문에 삭제 
        //tr.LookAt(target);
    }

    public void setTarget(Transform tr)
    {
        target = tr;
    }
}
