using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Projectile Spell Ability")]
public class ProjectileSpellAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public ProjectileSpellAbilityBehaviour abilityBehaviourPrefab;
    public List<StatusSO> AppliedStatuses;
    public int[] DMG_PerLevel;
    public int[] PROJ_SPEED_PerLevel;
    public int[] NUM_PROJ_PerLevel;
    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new ProjectileSpellRuntimeAbility(this);
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        throw new System.NotImplementedException();
    }

    public override string GetCurrentLevelDescription(int currentLevel)
    {
        string s = CurrentLevelDescription;

        float dam = DMG_PerLevel[currentLevel - 1];

        s = s.Replace("{DMG_CURR}", dam.ToString());

        float speed = PROJ_SPEED_PerLevel[currentLevel - 1];

        s = s.Replace("{PROJ_SPEED_CURR}", speed.ToString());

        int numProj = NUM_PROJ_PerLevel[currentLevel - 1];

        s = s.Replace("{NUM_PROJ_CURR}", numProj.ToString());

        return s;
    }

    public override string GetNextLevelDescription(int currentLevel)
    {
        string s = NextLevelDescription;

        float next = DMG_PerLevel[currentLevel];

        s = s.Replace("{DMG_NEXT}", next.ToString());

        next = PROJ_SPEED_PerLevel[currentLevel];

        s = s.Replace("{PROJ_SPEED_NEXT}", next.ToString());

        next = NUM_PROJ_PerLevel[currentLevel];

        s = s.Replace("{NUM_PROJ_NEXT}", next.ToString());

        return s;
    }
}
