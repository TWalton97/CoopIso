using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
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
        agent.isStopped = true;

        if (enemy.HasSpawnedItems)
        {
            animator.Play(DieHash, 0, 1f);
            animator.Update(0f);
        }
        else
        {
            animator.CrossFade(DieHash, 0f);
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
        int budget = 2 * (int)Mathf.Sqrt(enemy.EntityData.MaximumHealth);
        var loot = LootCalculator.RollItemsWithBudget(budget);

        foreach (var item in loot)
        {
            if (item.item is ConsumableDrop consumableDrop)
            {
                SpawnedItemDataBase.Instance.SpawnConsumableItem(consumableDrop, ReturnSpawnPositionInRadius(), Quaternion.identity);
            }
            else if (item.item is GoldDrop goldDrop)
            {
                GoldDrop drop = GameObject.Instantiate(goldDrop, ReturnSpawnPositionInRadius(), Quaternion.identity);
                drop.GoldAmount = item.amount;
                SceneManager.MoveGameObjectToScene(drop.gameObject, SceneLoadingManager.Instance.ReturnActiveEnvironmentalScene());
            }
            else
            {
                SpawnedItemDataBase.Instance.SpawnAndRegisterItem(item.itemData, ReturnSpawnPositionInRadius(), Quaternion.identity);
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
