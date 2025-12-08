[System.Serializable]
public abstract class RuntimeAbility
{
    public AbilitySO source;
    public int currentLevel;

    public PlayerResource.ResourceType resourceType;
    public float resourceAmount;

    protected RuntimeAbility(AbilitySO source)
    {
        this.source = source;
        currentLevel = 1;
        resourceType = source.ResourceType;
        resourceAmount = source.ResourceAmount;
    }

    public abstract void Upgrade();
}
