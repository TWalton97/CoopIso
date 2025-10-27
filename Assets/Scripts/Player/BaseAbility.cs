using UnityEngine;

public class BaseAbility : IAbility
{
    protected readonly NewPlayerController player;
    protected readonly ResourceController resourceController;
    protected readonly Animator animator;

    protected static readonly int AbilityHash = Animator.StringToHash("Ability");
    protected const float crossFadeDuration = 0.2f;

    public Resources.ResourceType ResourceType;
    public float ResourceAmount;

    protected BaseAbility(NewPlayerController player, ResourceController resourceController, Animator animator)
    {
        this.player = player;
        this.resourceController = resourceController;
        this.animator = animator;
    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual bool CanUse()
    {
        if (resourceController.resource.resourceType == ResourceType && resourceController.resource.RemoveResource(ResourceAmount)) return true;

        return false;
    }
}
