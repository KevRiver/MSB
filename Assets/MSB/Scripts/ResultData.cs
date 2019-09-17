using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultData : MonoBehaviour
{
    public enum Result { WIN,LOSE,DRAW}
    public Result result;
    public int kill;
    public int death;
    public int point;
}
