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

    public List<AbilitySO> UnlockedAbilities;

    public AbilityScrollController.AbilityData ActivatedAbility;

    public WeaponAbilityBehaviour weaponAbilityPrefab;

    private Dictionary<AbilitySO, RuntimeAbility> activeAbilities = new();
    private Dictionary<AbilitySO, AbilityBehaviourBase> behaviours = new();


    void Start()
    {
        newPlayerController.AnimationStatusTracker.OnAbilityCompleted += ExitActivatedAbility;
    }

    void OnDestroy()
    {
        newPlayerController.AnimationStatusTracker.OnAbilityCompleted -= ExitActivatedAbility;
    }

    public RuntimeAbility GetRuntime(AbilitySO so)
    {
        activeAbilities.TryGetValue(so, out var runtime);
        return runtime;
    }

    public AbilityBehaviourBase GetBehaviour(AbilitySO so)
    {
        behaviours.TryGetValue(so, out var behaviour);
        return behaviour;
    }

    public void UnlockAbility(AbilitySO abilitySO)
    {

        if (!activeAbilities.TryGetValue(abilitySO, out var runtime))
        {
            runtime = abilitySO.CreateRuntimeAbility();
            activeAbilities.Add(abilitySO, runtime);

            var behaviour = InstantiateBehaviourForAbility(abilitySO);

            if (abilitySO is WeaponAbility weaponAbility)
            {
                behaviour.Initialize(newPlayerController, runtime, weaponAbility.AppliedStatuses);
            }
            else
            {
                behaviour.Initialize(newPlayerController, runtime);
            }

            behaviours.Add(abilitySO, behaviour);

            newPlayerController.PlayerContext.UserInterfaceController.AddAbility(abilitySO, behaviour);

            UnlockedAbilities.Add(abilitySO);
            Debug.Log($"Unlocked ability {abilitySO.AbilityName}");
        }
        else
        {
            runtime.Upgrade();
            Debug.Log($"Upgraded ability {abilitySO.AbilityName} to level {runtime.currentLevel}");
        }
    }

    private AbilityBehaviourBase InstantiateBehaviourForAbility(AbilitySO so)
    {
        if (so is WeaponAbility weaponAbility)
            return Instantiate(weaponAbility.abilityBehaviourPrefab, transform);

        if (so is BuffAbility buffAbility)
            return Instantiate(buffAbility.buffBehaviourPrefab, transform);

        throw new Exception($"No behaviour prefab assigned for ability type: {so.GetType()}");
    }

    public void UseAbility(AbilityScrollController.AbilityData abilityData)
    {
        if (abilityData == null) return;

        newPlayerController.PlayerAnimationController.SetOverrideByPlaceholderName($"Ability_Default_Slot{1}", abilityData.AbilitySO.AnimationClip);
        ActivatedAbility = abilityData;
        abilityData.AbilityBehaviour.OnEnter();
        playerAnimator.SetInteger("AbilityIndex", 0);
        playerAnimator.SetTrigger("Cast");
    }

    private void ExitActivatedAbility()
    {
        if (ActivatedAbility == null) return;
        ActivatedAbility.AbilityBehaviour.OnExit();
        ActivatedAbility = null;
    }
}
