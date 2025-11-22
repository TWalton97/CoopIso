using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Weapon Ability")]
public class WeaponAbility : AbilitySO
{
    [Tooltip("Weapon abilities deal damage based on equipped weapons")] public float WeaponDamagePercentage;
}
