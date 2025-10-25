using System;
using UnityEngine;
using System.Collections.Generic;

public class ResourceController : MonoBehaviour
{
    public Resource resource;
}

[Serializable]
public class Resource
{
    public Resources.ResourceType resourceType;
    public float resourceMax;
    public float resourceCurrent;
    public float resourceMin = 0;

    public Action OnResourceMinReached;
    public Action OnResourceMaxReached;
    public Action OnResourceValueChanged;

    public Resource(Resources.ResourceType _resourceType, float _resourceMax, float _resourceCurrent, float _resourceMin = 0)
    {
        resourceType = _resourceType;
        resourceMax = _resourceMax;
        resourceCurrent = _resourceCurrent;
        resourceMin = _resourceMin;
    }

    public bool RemoveResource(float amount)
    {
        if (resourceCurrent - amount >= resourceMin)
        {
            resourceCurrent -= amount;
            OnResourceValueChanged?.Invoke();
            if (resourceCurrent == resourceMin)
            {
                OnResourceMinReached?.Invoke();
            }
            return true;
        }
        return false;
    }

    public void AddResource(float amount)
    {
        resourceCurrent = Mathf.Clamp(resourceCurrent + amount, resourceMin, resourceMax);
        OnResourceValueChanged?.Invoke();
        if (resourceCurrent == resourceMax)
        {
            OnResourceMaxReached?.Invoke();
        }
    }
}
