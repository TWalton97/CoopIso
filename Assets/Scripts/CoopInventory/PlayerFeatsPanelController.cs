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

    public ScrollRect scrollRect;
    public RectTransform contentPanel;

    private bool setupCompleted;

    public NewPlayerController playerController { get; private set; }

    public void CreateFeatButtons(FeatsController controller)
    {
        playerController = controller.newPlayerController;
        experienceController = controller.GetComponent<NewPlayerController>().ExperienceController;
        experienceController.OnLevelUp += UpdateSkillPointText;
        experienceController.OnSkillPointUsed += UpdateSkillPointText;
        UpdateSkillPointText();
        int index = 0;

        // foreach (Feat feat in controller.AvailableFeats)
        // {
        //     FeatButton featButton = Instantiate(FeatButtonPrefab, FeatBubbleParent);
        //     featButton.InitializeButton(feat, controller, this, index);
        //     featButtons.Add(featButton);

        //     index++;
        // }

        foreach (FeatSO feat in controller.AllFeats)
        {
            FeatButton featButton = Instantiate(FeatButtonPrefab, FeatBubbleParent);
            featButton.InitializeButton(feat, controller, this, index);
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
        FeatPreviewCost.text = "Cost: " + feat.GetCostPerLevel(currentFeatLevel).ToString();

        if (feat is StatIncreaseFeat statIncreaseFeat)
        {
            if (currentFeatLevel == feat.MaximumFeatLevel)
            {
                FeatPreviewStats.text = "Maximum Level Reached";
            }
            else
            {
                int safeIndex = Mathf.Clamp(currentFeatLevel, 0, statIncreaseFeat.ValueIncreasePerLevel.Length - 1);
                FeatPreviewStats.text = "Next Level: " + feat.FeatUpgradeDescription + statIncreaseFeat.ValueIncreasePerLevel[safeIndex];
            }
        }
        else if (feat is AbilityUnlockFeat abilityUnlockFeat)
        {
            if (currentFeatLevel == 0)
            {
                FeatPreviewStats.text = "Next Level: " + feat.FeatUnlockDescription;
            }
            else if (currentFeatLevel == feat.MaximumFeatLevel)
            {
                FeatPreviewStats.text = "Maximum level reached";
            }
            else
            {
                FeatPreviewStats.text = abilityUnlockFeat.GenerateStatDescriptionString(currentFeatLevel);
            }
        }

        FeatPreviewDescription.text = feat.FeatStatDescription;

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
