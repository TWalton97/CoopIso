using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AbilityController : MonoBehaviour
{
    public NewPlayerController newPlayerController;
    public ResourceController resourceController;

    public Animator playerAnimator;
    public AnimatorOverrideController abilityAOC;
    public Action OnAbilityStarted;
    public Action OnAbilityFinished;

    public List<BaseAbility> UnlockedAbilities;

    public BaseAbility AbilityToEquip;

    public BaseAbility equippedAbility1;
    public BaseAbility equippedAbility2;
    public BaseAbility equippedAbility3;

    public BaseAbility ActivatedAbility;

    void Start()
    {
        newPlayerController.AnimationStatusTracker.OnAbilityCompleted += ExitActivatedAbility;
    }

    void OnDestroy()
    {
        newPlayerController.AnimationStatusTracker.OnAbilityCompleted -= ExitActivatedAbility;
    }

    public void EquipAbility(int slot, BaseAbility ability)
    {
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName($"Ability_Default_Slot{slot + 1}", ability.abilityData.AnimationClip);
        equippedAbility1 = ability;
        equippedAbility1.Init(newPlayerController, newPlayerController.ResourceController);
    }

    public void UseAbility(BaseAbility ability)
    {
        if (ability == null) return;

        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName($"Ability_Default_Slot{1}", ability.abilityData.AnimationClip);
        ActivatedAbility = ability;
        ability.OnEnter(newPlayerController);
        playerAnimator.SetInteger("AbilityIndex", 0);
        playerAnimator.SetTrigger("Cast");
    }

    private void ExitActivatedAbility()
    {
        if (ActivatedAbility == null) return;
        ActivatedAbility.OnExit();
        ActivatedAbility = null;
    }

    public void UnlockAbility(BaseAbility ability)
    {
        UnlockedAbilities.Add(ability);
        InventoryManager.Instance.GetPlayerUserInterfaceControllerByIndex(newPlayerController.PlayerInputController.playerIndex).AddAbility(ability);
    }
}
