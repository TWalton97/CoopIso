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

    public BaseAbility AbilityToEquip;

    public BaseAbility equippedAbility1;
    public BaseAbility equippedAbility2;
    public BaseAbility equippedAbility3;

    public BaseAbility ActivatedAbility;

    void Start()
    {
        EquipAbility1();
        newPlayerController.AnimationStatusTracker.OnAbilityCompleted += ExitActivatedAbility;
    }

    void OnDestroy()
    {
        newPlayerController.AnimationStatusTracker.OnAbilityCompleted -= ExitActivatedAbility;
    }

    [ContextMenu("EquipAbility1")]
    public void EquipAbility1()
    {
        EquipAbility(0, AbilityToEquip);
    }

    public void EquipAbility(int slot, BaseAbility ability)
    {
        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName($"Ability_Default_Slot{slot + 1}", ability.abilityData.AnimationClip);
        equippedAbility1 = ability;
        equippedAbility1.Init(newPlayerController, newPlayerController.ResourceController);
    }

    [ContextMenu("Use Ability 1")]
    public void UseAbility1()
    {
        if (equippedAbility1 == null) return;

        ActivatedAbility = equippedAbility1;
        equippedAbility1.OnEnter();
        playerAnimator.SetInteger("AbilityIndex", 0);
        playerAnimator.SetTrigger("Cast");
    }

    private void ExitActivatedAbility()
    {
        if (ActivatedAbility == null) return;
        ActivatedAbility.OnExit();
        ActivatedAbility = null;
    }
}
