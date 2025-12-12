
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SummonAbilityBehaviour : AbilityBehaviour<SummonRuntimeAbility>
{
    public Enemy unitToSpawn;
    public int Health;
    public int MaximumNumberOfSummons;
    private List<Enemy> SummonedUnits = new();

    public override void OnEnter()
    {
        MaximumNumberOfSummons = runtime.MaxSummons;

        if (SummonedUnits.Count < MaximumNumberOfSummons)
        {
            Enemy unit = Instantiate(unitToSpawn, player.transform.position + player.transform.forward, Quaternion.identity);
            if (unit is FriendlySkeletonArcher archer)
            {
                archer.Init(player);
                archer.OnFriendlyArcherDied += RemoveUnitFromList;
            }
            else if (unit is FriendlySkeletonWarrior warrior)
            {
                warrior.Init(player);
                warrior.OnFriendlyWarriorDied += RemoveUnitFromList;
            }

            unit.HealthController.MaximumHealth = runtime.Health;
            unit.HealthController.CurrentHealth = runtime.Health;
            unit.HealthController.IncreaseMaximumHealth(0);

            unit.damage = runtime.Damage;

            SummonedUnits.Add(unit);
        }
        else
        {
            RemoveUnitFromList(SummonedUnits[0]);
            OnEnter();
        }
    }

    public override void OnChannelTick(float deltaTime)
    {

    }

    public override void OnExit()
    {

    }

    public override void OnUse()
    {

    }

    public void RemoveUnitFromList(Enemy unit)
    {
        SummonedUnits.Remove(unit);
        Destroy(unit.gameObject, 3f);

        if (unit is FriendlySkeletonArcher archer)
        {
            archer.OnFriendlyArcherDied -= RemoveUnitFromList;
        }
        else if (unit is FriendlySkeletonWarrior warrior)
        {
            warrior.OnFriendlyWarriorDied -= RemoveUnitFromList;
        }
    }
}
