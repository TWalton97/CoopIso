using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxSpawner : MonoBehaviour
{
    public PlayerController playerController;

    public void AttackCompleted()
    {
        playerController.ReduceAttackCounter();
    }
}
