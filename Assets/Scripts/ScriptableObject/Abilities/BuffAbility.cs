
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Buff Ability")]
public class BuffAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public StatusSO statusSO;
    public BuffAbilityBehaviour buffBehaviourPrefab;

    public float[] VAL_PerLevel;
    public float[] DUR_PerLevel;

    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new BuffRuntimeAbility(this);
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        throw new System.NotImplementedException();
    }

    public override string GetCurrentLevelDescription(int currentLevel)
    {
        string s = CurrentLevelDescription;

        float val = VAL_PerLevel[currentLevel - 1];
        float dur = DUR_PerLevel[currentLevel - 1];

        s = s.Replace("{VAL_CURR}", val.ToString());
        s = s.Replace("{DUR_CURR}", dur.ToString());

        return s;
    }

    public override string GetNextLevelDescription(int currentLevel)
    {
        string s = NextLevelDescription;

        float next = VAL_PerLevel[currentLevel];

        s = s.Replace("{VAL_NEXT}", next.ToString());

        next = DUR_PerLevel[currentLevel];

        s = s.Replace("{DUR_NEXT}", next.ToString());

        return s;
    }
}
