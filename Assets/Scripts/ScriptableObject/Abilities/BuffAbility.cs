
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Buff Ability")]
public class BuffAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public StatusSO statusSO;
    public BuffAbilityBehaviour buffBehaviourPrefab;
    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new BuffRuntimeAbility(this);
    }
}
