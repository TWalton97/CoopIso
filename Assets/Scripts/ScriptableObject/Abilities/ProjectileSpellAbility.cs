using UnityEngine;
[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Projectile Spell Ability")]
public class ProjectileSpellAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public ProjectileSpellAbilityBehaviour abilityBehaviourPrefab;
    public StatusSO[] AppliedStatuses;
    public int[] DamagePerLevel;
    public int[] ProjectileSpeed;
    public int[] NumberOfProjectiles;
    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new ProjectileSpellRuntimeAbility(this);
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        throw new System.NotImplementedException();
    }

    public override string GetLevelDescription(int currentLevel)
    {
        string s = LevelDescriptionTemplate;

        float dam = DamagePerLevel[currentLevel - 1];

        s = s.Replace("{DAM}", dam.ToString());

        float speed = ProjectileSpeed[currentLevel - 1];

        s = s.Replace("{PROJ_SPEED}", speed.ToString());

        int numProj = NumberOfProjectiles[currentLevel - 1];

        s = s.Replace("{NUM_PROJ}", numProj.ToString());

        return s;
    }

    public override string GetUpgradeDescription(int currentLevel)
    {
        string s = UpgradeDescriptionTemplate;

        float curr = DamagePerLevel[currentLevel - 1];
        float next = DamagePerLevel[currentLevel];

        s = s.Replace("{DAM_CURR}", curr.ToString());
        s = s.Replace("{DAM_NEXT}", next.ToString());

        curr = ProjectileSpeed[currentLevel - 1];
        next = ProjectileSpeed[currentLevel];

        s = s.Replace("{PROJ_SPEED_CURR}", curr.ToString());
        s = s.Replace("{PROJ_SPEED_NEXT}", next.ToString());

        curr = NumberOfProjectiles[currentLevel - 1];
        next = NumberOfProjectiles[currentLevel];

        s = s.Replace("{NUM_PROJ_CURR}", curr.ToString());
        s = s.Replace("{NUM_PROJ_NEXT}", next.ToString());

        return s;
    }
}
