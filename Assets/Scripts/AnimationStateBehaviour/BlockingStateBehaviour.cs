using UnityEngine;

public class BlockingStateBehaviour : StateMachineBehaviour
{
    public bool isBlockingOnEnter = true;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var player = animator.GetComponentInParent<NewPlayerController>();
        if (player != null && player.WeaponController.CanBlock)
        {
            player.IsBlocking = isBlockingOnEnter;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var player = animator.GetComponentInParent<NewPlayerController>();
        if (player != null)
        {
            player.IsBlocking = false;
        }
    }
}
