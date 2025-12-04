using UnityEngine;

public class GoldDrop : Item, IInteractable
{
    public override string GetInteractableName()
    {
        return Quantity + " Gold";
    }

    public override void OnInteract(PlayerContext playerContext, int playerIndex)
    {
        playerContext.PlayerController.PlayerStatsBlackboard.AddGold(Quantity);
        Destroy(gameObject);
    }
}
