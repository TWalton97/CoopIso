
using System.Collections.Generic;
using UnityEngine;

public class SummonAbilityBehaviour : AbilityBehaviour<SummonRuntimeAbility>
{
    public FriendlySkeletonArcher friendlySkeletonArcher;
    public int Health;
    public int MaximumNumberOfSummons;
    private List<FriendlySkeletonArcher> SummonedArchers = new();

    public override void OnEnter()
    {
        if (SummonedArchers.Count < MaximumNumberOfSummons)
        {
            FriendlySkeletonArcher archer = Instantiate(friendlySkeletonArcher, player.transform.position + player.transform.forward, Quaternion.identity);
            archer.Init(player);
            archer.HealthController.MaximumHealth = Health;
            archer.HealthController.CurrentHealth = Health;
            archer.HealthController.IncreaseMaximumHealth(0);

            archer.damage = runtime.Damage;

            SummonedArchers.Add(archer);
            archer.OnFriendlyArcherDied += RemoveArcherFromList;
        }
        else
        {
            RemoveArcherFromList(SummonedArchers[0]);
            OnEnter();
        }
    }

    public override void OnExit()
    {

    }

    public override void OnUse()
    {

    }

    public void RemoveArcherFromList(FriendlySkeletonArcher archer)
    {
        SummonedArchers.Remove(archer);
        Destroy(archer.gameObject);
        archer.OnFriendlyArcherDied -= RemoveArcherFromList;
    }
}
