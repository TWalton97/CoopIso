using UnityEngine;

public class StatusInstance
{
    public StatusSO data;
    public float remainingDuration;
    public int stacks;

    public GameObject spawnedVFX;

    public float tickTimer;

    public bool IsExpired => remainingDuration <= 0;

    public StatusInstance(StatusSO data)
    {
        this.data = data;
        remainingDuration = data.baseDuration;
        stacks = 1;
        tickTimer = 0;
    }
}
