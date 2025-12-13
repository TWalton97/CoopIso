public class BuffRuntimeAbility : RuntimeAbility
{
    private BuffAbility data;
    public float BuffAmount;
    public float Duration;

    public BuffRuntimeAbility(BuffAbility data) : base(data)
    {
        this.data = data;
        BuffAmount = data.VAL_PerLevel[currentLevel - 1];
        Duration = data.DUR_PerLevel[currentLevel - 1];
    }

    public override void Upgrade()
    {
        currentLevel++;
        BuffAmount = data.VAL_PerLevel[currentLevel - 1];
        Duration = data.DUR_PerLevel[currentLevel - 1];
    }
}
