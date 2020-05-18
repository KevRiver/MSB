using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MSB_View<T> where T : struct
{
    void ApplyData(T t);
}
