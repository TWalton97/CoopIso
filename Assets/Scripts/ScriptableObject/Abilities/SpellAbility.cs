using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Data/Ability Data/Spell Ability")]
public class SpellAbility : AbilitySO
{
    public int AbilityDamage;
    public int AbilityDamageIncreasePerLevel;
    public override RuntimeAbility CreateRuntimeAbility()
    {
        return new SpellRuntimeAbility(this);
    }
}
