using UnityEngine;

public class BaseAbility : MonoBehaviour, IAbility
{
    protected NewPlayerController player;
    protected ResourceController resourceController;

    public AbilitySO abilityData;
    protected BaseAbility(NewPlayerController player, ResourceController resourceController)
    {
        this.player = player;
        this.resourceController = resourceController;
    }

    public void Init(NewPlayerController _player, ResourceController _resourceController)
    {
        player = _player;
        resourceController = _resourceController;
    }

    public virtual void OnEnter(NewPlayerController controller)
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual bool CanUse(ResourceController resourceController)
    {
        if (resourceController.resource.resourceType == abilityData.ResourceType && resourceController.resource.RemoveResource(abilityData.ResourceAmount)) return true;

        return false;
    }
}
