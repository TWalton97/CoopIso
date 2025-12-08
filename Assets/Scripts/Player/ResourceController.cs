using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ResourceController : MonoBehaviour
{
    public NewPlayerController newPlayerController;
    public Resource resource;

    void Start()
    {
        StartCoroutine(RegenerateResource());
    }

    private IEnumerator RegenerateResource()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            resource.AccumulatedRegen += resource.RegenPerSecond * 0.1f;

            if (resource.AccumulatedRegen >= 1f)
            {
                int gained = Mathf.FloorToInt(resource.AccumulatedRegen);
                resource.AccumulatedRegen -= gained;
                resource.AddResource(gained);
            }
        }
    }

    public IEnumerator RestoreResourceOverDuration(int amountOfResource, int duration)
    {
        resource.remainingRestoreAmount = amountOfResource;
        float accumulatedRegen = 0f;
        float resourcePerSecond = amountOfResource / duration;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(0.1f);
            elapsedTime += 0.1f;
            accumulatedRegen += resourcePerSecond * 0.1f;
            if (accumulatedRegen >= 1f)
            {
                int gained = Mathf.FloorToInt(accumulatedRegen);
                resource.remainingRestoreAmount -= gained;
                accumulatedRegen -= gained;
                resource.AddResource(gained);
            }
        }
    }
}

[Serializable]
public class Resource
{
    public PlayerResource.ResourceType resourceType;
    public float resourceMax;
    public float resourceCurrent;
    public float resourceMin = 0;

    public float RegenPerSecond;
    public float AccumulatedRegen;

    public Action OnResourceMinReached;
    public Action OnResourceMaxReached;
    public Action OnResourceValueChanged;

    public float remainingRestoreAmount;

    public Resource(PlayerResource.ResourceType _resourceType, float _resourceMax, float _resourceCurrent, float _resourceMin = 0)
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
