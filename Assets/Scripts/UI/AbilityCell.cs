using UnityEngine;
using UnityEngine.UI;

public class AbilityCell : MonoBehaviour
{
    public Image icon;

    public void SetAbility(BaseAbility ability)
    {
        icon.sprite = ability.abilityData.AbilityIcon;
    }
}
