using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyDieState : EnemyBaseState
{
    readonly NavMeshAgent agent;
    readonly Transform player;
    public EnemyDieState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        agent.enabled = false;

        if (enemy.HasSpawnedItems)
        {
            animator.Play(DieHash, 0, 1f);
            animator.Update(0f);
        }
        else
        {
            animator.CrossFade(DieHash, crossFadeDuration);
            enemy.StartCoroutine(SpawnItems());
            enemy.HasSpawnedItems = true;
        }

        enemy.coll.enabled = false;

        if (enemy.statusController != null)
        {
            enemy.statusController.RemoveAllStatuses();
        }
    }

    public override void Update()
    {

    }

    private IEnumerator SpawnItems()
    {
        int budget = enemy.EntityData.MaximumHealth / 10;
        var loot = LootCalculator.RollItemsWithBudget(budget);

        foreach (var item in loot)
        {
            if (item.item is ConsumableDrop consumableDrop)
            {
                SpawnedItemDataBase.Instance.SpawnConsumableItem(consumableDrop, ReturnSpawnPositionInRadius(), Quaternion.identity);
            }
            else
            {
                SpawnedItemDataBase.Instance.SpawnAndRegisterItem(item.item.itemData, ReturnSpawnPositionInRadius(), Quaternion.identity);
            }
            yield return new WaitForSeconds(0.2f);
        }
        yield return null;
    }

    private Vector3 ReturnSpawnPositionInRadius()
    {
        Vector3 insideUnitCircle = Random.insideUnitCircle;
        insideUnitCircle = new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
        return enemy.transform.position + insideUnitCircle;
    }
}
