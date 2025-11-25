public class BuffRuntimeAbility : RuntimeAbility
{
    private BuffAbility data;
    public float BuffAmount;
    public float Duration;

    public BuffRuntimeAbility(BuffAbility data) : base(data)
    {
        this.data = data;
        BuffAmount = data.BuffValuePerLevel[currentLevel - 1];
        Duration = data.BuffValuePerLevel[currentLevel - 1];
    }

    public override void Upgrade()
    {
        currentLevel++;
        BuffAmount = data.BuffValuePerLevel[currentLevel - 1];
        Duration = data.BuffValuePerLevel[currentLevel - 1];
    }
}
