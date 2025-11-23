using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Weapon Ability")]
public class WeaponAbility : AbilitySO
{
    public WeaponRangeType RequiredWeaponRangeType;
    public WeaponAbilityBehaviour abilityBehaviourPrefab;
    [Tooltip("Weapon abilities deal damage based on equipped weapons")] public float WeaponDamagePercentage;
    [Tooltip("Weapon ability damage increase per level")] public float WeaponDamageIncreasePerLevel;
    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new WeaponRuntimeAbility(this);
    }
}
