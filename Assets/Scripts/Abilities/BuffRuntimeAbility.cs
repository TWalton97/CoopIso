public class BuffRuntimeAbility : RuntimeAbility
{
    public float BuffAmount;
    public float Duration;

    public BuffRuntimeAbility(BuffAbility data) : base(data)
    {

    }

    public override void Upgrade()
    {
        currentLevel++;
    }
}
