using UnityEngine;
using System;

public class AnimationEventHandler : MonoBehaviour
{
    public event Action OnFinish;
    public event Action OnStartMovement;
    public event Action OnStopMovement;

    public void AnimationFinishedTrigger() => OnFinish?.Invoke();
    public void StartMovementTrigger() => OnStartMovement?.Invoke();
    public void StopMovementTrigger() => OnStopMovement?.Invoke();

}
