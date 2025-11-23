using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatData", menuName = "Data/Feat Data/Passive Unlock Feat")]

public class PassiveUnlockFeat : FeatSO
{
    public PassiveUnlock passiveUnlock;

    public override void OnActivate(int CurrentFeatLevel, NewPlayerController controller, Action activatedSuccess)
    {
        UnlockPassive(controller);
        activatedSuccess?.Invoke();
    }

    private void UnlockPassive(NewPlayerController controller)
    {
        switch (passiveUnlock)
        {
            case PassiveUnlock.CanEquipShields:
                controller.PlayerStatsBlackboard.CanEquipShields = true;
                break;
            case PassiveUnlock.UnarmedMastery:
                controller.PlayerStatsBlackboard.UnarmedMastery = true;
                break;
            case PassiveUnlock.OneHandedMastery:
                controller.PlayerStatsBlackboard.OneHandedMastery = true;
                break;
            case PassiveUnlock.TwoHandedMastery:
                controller.PlayerStatsBlackboard.TwoHandedMastery = true;
                break;
            case PassiveUnlock.DualWieldMastery:
                controller.PlayerStatsBlackboard.DualWieldMastery = true;
                break;
            case PassiveUnlock.BowMastery:
                controller.PlayerStatsBlackboard.BowMastery = true;
                break;
        }
    }
}

public enum PassiveUnlock
{
    CanEquipShields,
    UnarmedMastery,
    OneHandedMastery,
    TwoHandedMastery,
    DualWieldMastery,
    BowMastery
}
