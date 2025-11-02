using UnityEngine;

public class PotionController : MonoBehaviour
{
    public HealthController HealthController;

    public bool drinkingPotion = false;

    public void UsePotion(PotionSO PotionToUse)
    {
        if (drinkingPotion) return;

        for (int i = 0; i < PotionToUse.PotionData.Count; i++)
        {
            PotionData data = PotionToUse.PotionData[i];
            if (data.ResourceToRestore == Resources.ResourceType.Health)
            {
                drinkingPotion = true;
                StartCoroutine(HealthController.RestoreHealthOverDuration(data.AmountOfResourceToRestore, data.RestoreDuration, () => drinkingPotion = false));
            }
            else if (data.ResourceToRestore == Resources.ResourceType.Mana)
            {
                Debug.Log("Restoring mana but it's not setup yet...");
            }
        }

    }
}
