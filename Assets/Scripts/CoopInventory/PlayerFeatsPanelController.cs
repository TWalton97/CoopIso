using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerFeatsPanelController : MonoBehaviour
{
    public Button LeftTab;
    public Button RightTab;

    public TMP_Text SkillPointsRemainingText;
    public FeatButton FeatButtonPrefab;
    public Transform FeatBubbleParent;

    public List<FeatButton> featButtons = new();

    private ExperienceController experienceController;

    public TMP_Text FeatPreviewTitle;
    public TMP_Text FeatPreviewCost;
    public TMP_Text FeatPreviewStats;
    public TMP_Text FeatPreviewDescription;
    public TMP_Text FeatPreviewWeaponRequirement;
    public TMP_Text FeatPreviewResourceCost;

    public ScrollRect scrollRect;
    public RectTransform contentPanel;

    private bool setupCompleted;

    public NewPlayerController playerController { get; private set; }

    public void CreateFeatButtons(NewPlayerController controller)
    {
        playerController = controller;
        experienceController = controller.ExperienceController;

        experienceController.OnLevelUp += UpdateSkillPointText;
        experienceController.OnSkillPointUsed += UpdateSkillPointText;

        UpdateSkillPointText();
        int index = 0;

        foreach (FeatSO feat in controller.FeatsController.AllFeats)
        {
            FeatButton featButton = Instantiate(FeatButtonPrefab, FeatBubbleParent);
            featButton.InitializeButton(feat, controller.FeatsController, this, index);
            featButtons.Add(featButton);

            index++;
        }

        Navigation rightTabNavigation = new Navigation();
        rightTabNavigation.mode = Navigation.Mode.Explicit;
        rightTabNavigation.selectOnLeft = LeftTab;
        rightTabNavigation.selectOnDown = featButtons[0].GetComponent<Button>();
        RightTab.navigation = rightTabNavigation;

        Navigation leftTabNavigation = new Navigation();
        leftTabNavigation.mode = Navigation.Mode.Explicit;
        leftTabNavigation.selectOnRight = RightTab;
        leftTabNavigation.selectOnDown = featButtons[0].GetComponent<Button>();
        LeftTab.navigation = leftTabNavigation;
    }

    public void OnEnable()
    {
        UpdateFeatPreviewWindow(featButtons[0].currentFeatLevel, featButtons[0].feat);
        UpdateSkillPointText();
        foreach (FeatButton featButton in featButtons)
        {
            featButton.CheckIfPlayerHasEnoughSkillpoints();
        }

        if (!setupCompleted)
        {
            setupCompleted = true;
            return;
        }
        UpdateViewPosition(featButtons[0].rectTransform);
    }

    public void OnDestroy()
    {
        experienceController.OnLevelUp -= UpdateSkillPointText;
        experienceController.OnSkillPointUsed -= UpdateSkillPointText;
    }

    public void UpdateSkillPointText()
    {
        SkillPointsRemainingText.text = "Skill Points Remaining: " + experienceController.SkillPoints.ToString();
    }

    public void UpdateFeatPreviewWindow(int currentFeatLevel, FeatSO feat)
    {
        FeatPreviewTitle.text = feat.FeatName;
        FeatPreviewCost.text = "Cost:" + feat.GetCost(currentFeatLevel).ToString();
        FeatPreviewStats.text = feat.GetNextLevelEffect(currentFeatLevel);

        FeatPreviewDescription.text = feat.GetCurrentEffect(currentFeatLevel);

        if (feat is AbilityUnlockFeat abilityUnlockFeat)
        {
            FeatPreviewResourceCost.text = "Mana: " + abilityUnlockFeat.AbilityToUnlock.ResourceAmount.ToString();
            if (abilityUnlockFeat.AbilityToUnlock is WeaponAbility weaponAbility)
            {
                FeatPreviewWeaponRequirement.text = weaponAbility.RequiredWeaponRangeType.ToString();
            }
            else
            {
                FeatPreviewWeaponRequirement.text = "";
            }
        }
        else
        {
            FeatPreviewResourceCost.text = "";
        }
        // FeatPreviewTitle.text = feat.FeatName;
        // FeatPreviewCost.text = "Cost: " + feat.GetCostPerLevel(currentFeatLevel).ToString();

        // if (feat is StatIncreaseFeat statIncreaseFeat)
        // {
        //     if (currentFeatLevel == feat.MaximumFeatLevel)
        //     {
        //         FeatPreviewStats.text = "Maximum Level Reached";
        //     }
        //     else
        //     {
        //         int safeIndex = Mathf.Clamp(currentFeatLevel, 0, statIncreaseFeat.ValueIncreasePerLevel.Length - 1);
        //         FeatPreviewStats.text = "Next Level: " + feat.FeatUpgradeDescription + statIncreaseFeat.ValueIncreasePerLevel[safeIndex];
        //     }
        //     FeatPreviewResourceCost.text = "";
        //     FeatPreviewWeaponRequirement.text = "";
        // }
        // else if (feat is AbilityUnlockFeat abilityUnlockFeat)
        // {
        //     if (currentFeatLevel == 0)
        //     {
        //         FeatPreviewStats.text = "Next Level: " + feat.FeatUnlockDescription;
        //     }
        //     else if (currentFeatLevel == feat.MaximumFeatLevel)
        //     {
        //         FeatPreviewStats.text = "Maximum level reached";
        //     }
        //     else
        //     {
        //         FeatPreviewStats.text = abilityUnlockFeat.GenerateStatDescriptionString(currentFeatLevel);
        //     }

        //     if (abilityUnlockFeat.AbilityToUnlock is WeaponAbility weaponAbility && weaponAbility.RequiredWeaponRangeType != WeaponRangeType.None)
        //     {
        //         FeatPreviewWeaponRequirement.text = weaponAbility.RequiredWeaponRangeType.ToString();
        //     }
        //     else if (abilityUnlockFeat.AbilityToUnlock is BuffAbility buffAbility && buffAbility.RequiredWeaponRangeType != WeaponRangeType.None)
        //     {
        //         FeatPreviewWeaponRequirement.text = buffAbility.RequiredWeaponRangeType.ToString();
        //     }
        //     else
        //     {
        //         FeatPreviewWeaponRequirement.text = "";
        //     }

        //     FeatPreviewDescription.text = GetFullDescription(abilityUnlockFeat);
        //     FeatPreviewResourceCost.text = abilityUnlockFeat.AbilityToUnlock.ResourceAmount.ToString() + " " + abilityUnlockFeat.AbilityToUnlock.ResourceType.ToString();
        //     return;
        // }
        // else if (feat is PassiveUnlockFeat)
        // {
        //     if (currentFeatLevel == feat.MaximumFeatLevel)
        //     {
        //         FeatPreviewStats.text = "Maximum Level Reached";
        //     }
        //     else
        //     {
        //         FeatPreviewStats.text = "Next Level: " + feat.FeatUnlockDescription;
        //     }
        //     FeatPreviewResourceCost.text = "";
        //     FeatPreviewWeaponRequirement.text = "";
        // }

        // FeatPreviewDescription.text = feat.FeatStatDescription;
    }

    public void UpdateViewPosition(RectTransform target)
    {
        //Only do this if the player is using gamepad
        if (playerController.PlayerInputController.playerInput.currentControlScheme == playerController.KEYBOARD_SCHEME) return;
        scrollRect.content.localPosition = GetSnapToPositionToBringChildIntoView(scrollRect, target);
    }

    public Vector2 GetSnapToPositionToBringChildIntoView(ScrollRect instance, RectTransform child)
    {
        Canvas.ForceUpdateCanvases();
        Vector2 viewportLocalPosition = instance.viewport.localPosition;
        Vector2 childLocalPosition = child.localPosition;
        Vector2 result = new Vector2(
            0 - (viewportLocalPosition.x + childLocalPosition.x),
            0 - (viewportLocalPosition.y + childLocalPosition.y)
        );
        return result;
    }

    // public string GetFullDescription(AbilityUnlockFeat abilityUnlockFeat)
    // {
    //     string text = abilityUnlockFeat.FeatStatDescription; // “Performs a melee slash, damaging all hit enemies for 100% of weapon damage”

    //     if (abilityUnlockFeat.AbilityToUnlock is WeaponAbility weaponAbility)
    //     {
    //         if (weaponAbility.AppliedStatuses != null && weaponAbility.AppliedStatuses.Count > 0)
    //         {
    //             text += " and applying ";
    //             for (int i = 0; i < weaponAbility.AppliedStatuses.Count; i++)
    //             {
    //                 var status = weaponAbility.AppliedStatuses[i];
    //                 // Wrap with TMP <link> instead of <status>
    //                 text += $"<link=\"{status.statusID}\"><color=#FF0000>{status.statusID}</color></link>";
    //                 if (i < weaponAbility.AppliedStatuses.Count - 1)
    //                     text += ", ";
    //             }
    //         }
    //     }
    //     return text + ".";
    // }
}
