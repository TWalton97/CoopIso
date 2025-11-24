using UnityEngine;

[System.Serializable]
public class StatusInstance
{
    public StatusSO data;
    public float remainingDuration;
    public int stacks;

    public GameObject spawnedVFX;

    public float tickTimer;

    public int initialHitDamage;

    public Entity source;

    public bool IsExpired => remainingDuration <= 0;

    public StatusInstance(StatusSO data, Entity source)
    {
        this.data = data;
        this.source = source;
        remainingDuration = data.baseDuration;
        stacks = 1;
        tickTimer = 0;
    }
}
