using UnityEngine;

public class DamageNumberManager : Singleton<DamageNumberManager>
{
    public DamageNumber damageNumberPrefab;

    public void SpawnNumber(int amount, Vector3 worldPosition, bool isCritical = false)
    {
        DamageNumber num = Instantiate(
            damageNumberPrefab,
            worldPosition,
            Quaternion.identity
        );

        num.Initialize(amount, isCritical);
    }
}

