using UnityEngine;

public class GoldDrop : Item, IInteractable
{
    public int GoldAmount;

    public override string GetInteractableName()
    {
        return GoldAmount.ToString() + " Gold";
    }

    public override void OnInteract(PlayerContext playerContext, int playerIndex)
    {
        playerContext.PlayerController.PlayerStatsBlackboard.AddGold(GoldAmount);
        Destroy(gameObject);
    }
}
