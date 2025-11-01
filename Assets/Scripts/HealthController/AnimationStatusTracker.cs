using System;
using UnityEngine;

public class AnimationStatusTracker : MonoBehaviour
{
    public Action OnAnimationCompleted;

    public void AnimationCompleted()
    {
        OnAnimationCompleted?.Invoke();
    }
}
