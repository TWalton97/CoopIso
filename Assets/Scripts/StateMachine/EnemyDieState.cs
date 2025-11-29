using System.Collections;
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
        int numItemsToSpawn = SpawnedItemDataBase.Instance.GetAffixCount(enemy.HealthController.MaximumHealth);
        for (int i = 0; i < numItemsToSpawn; i++)
        {
            Item instantiatedItem = SpawnedItemDataBase.Instance.SpawnRandomItem(enemy.HealthController.MaximumHealth, null, enemy.transform);
            instantiatedItem.transform.position = ReturnSpawnPositionInRadius();
            yield return new WaitForSeconds(0.2f);
        }

        if (Random.Range(0, 5) < 2)
        {
            ConsumableDrop consumableDrop = GameObject.Instantiate(SpawnedItemDataBase.Instance.spawnableConsumables[Random.Range(0, 2)], enemy.transform.position, Quaternion.identity, enemy.transform);
            consumableDrop.transform.position = ReturnSpawnPositionInRadius();
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
