public class NavMeshChaseState : BaseState
{
    BaseNavMeshUnitController navMeshController;
    public NavMeshChaseState(BaseUnitController _controller) : base(_controller)
    {
        navMeshController = _controller as BaseNavMeshUnitController;
    }

    public override void Update()
    {

        //If the target leaves the LEASH range, continue moving until you reach your current destination
        //Then return to looking for a target

        if (navMeshController.IsTargetStillInRange())
        {
            navMeshController.SetAgentDestination(navMeshController.Target.transform.position);
        }
        else if (navMeshController.Agent.remainingDistance <= navMeshController.Agent.stoppingDistance)
        {
            navMeshController.FindTarget();
        }
    }
}
