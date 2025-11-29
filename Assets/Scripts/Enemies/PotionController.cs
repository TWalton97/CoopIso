using UnityEngine;

public class PotionController : MonoBehaviour
{
    public HealthController HealthController;
    public ResourceController ResourceController;

    public bool drinkingPotion = false;

    public void UsePotion(PotionSO potionData)
    {
        if (drinkingPotion) return;

        if (potionData.ResourceToRestore == Resources.ResourceType.Health)
        {
            drinkingPotion = true;
            StartCoroutine(HealthController.RestoreHealthOverDuration(potionData.AmountOfResourceToRestore, potionData.RestoreDuration, () => drinkingPotion = false));
        }
        else if (potionData.ResourceToRestore == Resources.ResourceType.Mana)
        {
            StartCoroutine(ResourceController.RestoreResourceOverDuration(potionData.AmountOfResourceToRestore, potionData.RestoreDuration, () => drinkingPotion = false));
        }
    }
}
