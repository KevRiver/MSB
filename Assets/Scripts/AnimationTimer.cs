using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class AnimationTimer
{
    public static IEnumerator TriggerAfter(Animator animator, string trigger, float t)
    {
        yield return new WaitForSeconds(t);
        animator.SetTrigger(trigger);
    }
}

