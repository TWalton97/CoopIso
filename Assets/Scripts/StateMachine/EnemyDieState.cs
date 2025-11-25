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
        foreach (GameObject obj in enemy.body)
        {
            obj.SetActive(false);
        }
        enemy.ragdoll.SetActive(true);
        enemy.coll.enabled = false;
        enemy.StartCoroutine(SpawnItems());

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
        int numItemsToSpawn = SpawnedItemDataBase.Instance.GetAffixCount(enemy.healthController.MaximumHealth);
        for (int i = 0; i < numItemsToSpawn; i++)
        {
            Item instantiatedItem = SpawnedItemDataBase.Instance.SpawnRandomItem(enemy.healthController.MaximumHealth);
            instantiatedItem.transform.position = ReturnSpawnPositionInRadius();
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
