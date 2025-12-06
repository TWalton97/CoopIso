public class ProjectileSpellRuntimeAbility : RuntimeAbility
{
    public ProjectileSpellAbility data;
    public StatusSO[] Statuses;
    public int Damage;
    public int ProjectileSpeed;
    public int NumberOfProjectiles;

    public ProjectileSpellRuntimeAbility(ProjectileSpellAbility data) : base(data)
    {
        this.data = data;
        Damage = data.DamagePerLevel[currentLevel - 1];
        ProjectileSpeed = data.ProjectileSpeed[currentLevel - 1];
        NumberOfProjectiles = data.NumberOfProjectiles[currentLevel - 1];
        Statuses = data.AppliedStatuses;
    }

    public override void Upgrade()
    {
        currentLevel++;
        Damage = data.DamagePerLevel[currentLevel - 1];
        ProjectileSpeed = data.ProjectileSpeed[currentLevel - 1];
        NumberOfProjectiles = data.NumberOfProjectiles[currentLevel - 1];
    }
}
