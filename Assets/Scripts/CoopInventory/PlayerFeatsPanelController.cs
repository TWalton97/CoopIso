using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerFeatsPanelController : MonoBehaviour
{
    public PlayerContext PlayerContext;
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

    public void CreateFeatButtons(PlayerContext playerContext)
    {
        PlayerContext = playerContext;
        playerController = playerContext.PlayerController;
        experienceController = playerController.ExperienceController;

        experienceController.OnLevelUp += UpdateSkillPointText;
        experienceController.OnSkillPointUsed += UpdateSkillPointText;

        UpdateSkillPointText();
        int index = 0;

        foreach (FeatSO feat in playerController.FeatsController.AllFeats)
        {
            FeatButton featButton = Instantiate(FeatButtonPrefab, FeatBubbleParent);
            featButton.InitializeButton(feat, playerController.FeatsController, this, index);
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
        PlayerContext.UserInterfaceController.eventSystem.SetSelectedGameObject(FeatBubbleParent.GetChild(0).gameObject);
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
}
