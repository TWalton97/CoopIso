using UnityEngine;
using System;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnFinish;

    public void AnimationFinishedTrigger() => OnFinish?.Invoke();

}
