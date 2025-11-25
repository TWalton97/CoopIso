
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Buff Ability")]
public class BuffAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public StatusSO statusSO;
    public BuffAbilityBehaviour buffBehaviourPrefab;

    public float[] BuffValuePerLevel;
    public float[] BuffDurationPerLevel;

    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new BuffRuntimeAbility(this);
    }

    public override string GetCalculatedLevelDescription(int currentLevel, int damage)
    {
        throw new System.NotImplementedException();
    }

    public override string GetLevelDescription(int currentLevel)
    {
        string s = LevelDescriptionTemplate;

        float val = BuffValuePerLevel[currentLevel - 1];
        float dur = BuffDurationPerLevel[currentLevel - 1];

        s = s.Replace("{VAL}", val.ToString());
        s = s.Replace("{DUR}", dur.ToString());

        return s;
    }

    public override string GetUpgradeDescription(int currentLevel)
    {
        string s = UpgradeDescriptionTemplate;

        float curr = BuffValuePerLevel[currentLevel - 1];
        float next = BuffValuePerLevel[currentLevel];

        s = s.Replace("{VAL_CURR}", curr.ToString());
        s = s.Replace("{VAL_NEXT}", next.ToString());

        curr = BuffDurationPerLevel[currentLevel - 1];
        next = BuffDurationPerLevel[currentLevel];

        s = s.Replace("{DUR_CURR}", curr.ToString());
        s = s.Replace("{DUR_NEXT}", next.ToString());

        return s;
    }
}
