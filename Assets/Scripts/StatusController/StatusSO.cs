using System;
using UnityEngine;

public abstract class StatusSO : ScriptableObject
{
    [Header("General")]
    public string statusID;
    public float baseDuration = 5f;
    public bool isStackable = false;
    public bool refreshDurationOnReapply = true;


    public virtual void OnEnter(StatusInstance instance, StatusController target)
    {
    }

    public virtual void OnTick(StatusInstance instance, StatusController target, float deltaTime)
    {

    }

    public virtual void OnExit(StatusInstance instance, StatusController target)
    {
    }
}
