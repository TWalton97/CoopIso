using System.Collections.Generic;

public class ProjectileSpellRuntimeAbility : RuntimeAbility
{
    public ProjectileSpellAbility data;
    public List<StatusSO> Statuses;
    public int Damage;
    public int ProjectileSpeed;
    public int NumberOfProjectiles;

    public ProjectileSpellRuntimeAbility(ProjectileSpellAbility data) : base(data)
    {
        this.data = data;
        Damage = data.DMG_PerLevel[currentLevel - 1];
        ProjectileSpeed = data.PROJ_SPEED_PerLevel[currentLevel - 1];
        NumberOfProjectiles = data.NUM_PROJ_PerLevel[currentLevel - 1];
        Statuses = data.AppliedStatuses;
    }

    public override void Upgrade()
    {
        currentLevel++;
        Damage = data.DMG_PerLevel[currentLevel - 1];
        ProjectileSpeed = data.PROJ_SPEED_PerLevel[currentLevel - 1];
        NumberOfProjectiles = data.NUM_PROJ_PerLevel[currentLevel - 1];
    }
}
