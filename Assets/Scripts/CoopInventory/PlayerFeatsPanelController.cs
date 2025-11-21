using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerFeatsPanelController : MonoBehaviour
{
    public TMP_Text SkillPointsRemainingText;
    public FeatButton FeatButtonPrefab;
    public Transform FeatBubbleParent;

    public List<FeatButton> featButtons = new();

    private ExperienceController experienceController;

    public TMP_Text FeatPreviewTitle;
    public TMP_Text FeatPreviewCost;
    public TMP_Text FeatPreviewStats;
    public TMP_Text FeatPreviewDescription;

    public void CreateFeatButtons(FeatsController controller)
    {
        experienceController = controller.GetComponent<NewPlayerController>().ExperienceController;
        experienceController.OnLevelUp += UpdateSkillPointText;
        experienceController.OnSkillPointUsed += UpdateSkillPointText;
        UpdateSkillPointText();
        int index = 0;

        foreach (Feat feat in controller.AvailableFeats)
        {
            FeatButton featButton = Instantiate(FeatButtonPrefab, FeatBubbleParent);
            featButton.InitializeButton(feat, controller, this, index);
            featButtons.Add(featButton);

            index++;
        }
    }

    public void OnEnable()
    {
        UpdateFeatPreviewWindow(featButtons[0].feat);
        UpdateSkillPointText();
        foreach (FeatButton featButton in featButtons)
        {
            featButton.CheckIfPlayerHasEnoughSkillpoints();
        }
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

    public void UpdateFeatPreviewWindow(Feat feat)
    {
        FeatPreviewTitle.text = feat.FeatName;
        FeatPreviewCost.text = "Cost: " + feat.SkillPointsCostPerLevel.ToString();
        FeatPreviewStats.text = feat.GenerateStatString();
        FeatPreviewDescription.text = feat.FeatDescription;
    }
}
