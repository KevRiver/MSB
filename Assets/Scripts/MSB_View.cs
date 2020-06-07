using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MSB_View<T> where T : struct
{
    /// <summary>
    /// 뷰의 UI 컴포넌트에 data를 적용합니다
    /// </summary>
    /// <param name="t"></param>
    void ApplyData(T t);
    /// <summary>
    /// 뷰를 초기화 합니다
    /// </summary>
    /// <param name="t"></param>
    void Initialize(T t);
}
