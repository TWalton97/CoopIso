using UnityEngine;

public class PotionController : MonoBehaviour
{
    public HealthController HealthController;
    public ResourceController ResourceController;

    public void UsePotion(PotionSO potionData)
    {
        if (potionData.ResourceToRestore == PlayerResource.ResourceType.Health)
        {
            StartCoroutine(HealthController.RestoreHealthOverDuration(potionData.AmountOfResourceToRestore, potionData.RestoreDuration));
        }
        else if (potionData.ResourceToRestore == PlayerResource.ResourceType.Mana)
        {
            StartCoroutine(ResourceController.RestoreResourceOverDuration(potionData.AmountOfResourceToRestore, potionData.RestoreDuration));
        }
    }
}
