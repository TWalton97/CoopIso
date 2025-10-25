using UnityEngine.AI;
using UnityEngine;

public class NavMeshIdleState : BaseState
{
    private const float WANDER_RADIUS = 5f;
    private const float WANDER_INTERVAL = 8f;
    private float elapsedTime = 0f;

    private BaseNavMeshUnitController navMeshController;
    public NavMeshIdleState(BaseUnitController _controller) : base(_controller)
    {
        navMeshController = _controller as BaseNavMeshUnitController;
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > WANDER_INTERVAL)
        {
            ChooseWanderDestination();
            elapsedTime = 0f;
        }

        if (!navMeshController.HasTarget())
        {
            navMeshController.FindTarget();
        }
    }

    private void ChooseWanderDestination()
    {
        Vector3 newPos = NavMeshUtils.RandomNavSphere(navMeshController.transform.position, WANDER_RADIUS, -1);
        navMeshController.Agent.SetDestination(newPos);
    }

}
