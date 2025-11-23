using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FeatButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    public Button button;
    public RectTransform rectTransform;
    public TMP_Text FeatName;
    public List<LevelBubble> FeatBubbles;
    private FeatsController controller;
    private ExperienceController experienceController;
    private PlayerFeatsPanelController playerFeatsPanelController;
    public int FeatIndex;
    public Button selectable;
    public FeatSO feat;
    public Color DeactiveColor;
    public Color ActiveColor;

    public int currentFeatLevel;

    public void InitializeButton(FeatSO _feat, FeatsController _controller, PlayerFeatsPanelController _playerFeatsPanelController, int _featIndex)
    {
        FeatIndex = _featIndex;
        controller = _controller;
        playerFeatsPanelController = _playerFeatsPanelController;
        feat = _feat;
        FeatName.text = feat.FeatName;
        button.onClick.AddListener(ActivateButton);
        for (int i = 0; i < feat.MaximumFeatLevel; i++)
        {
            FeatBubbles[i].gameObject.SetActive(true);
        }

        experienceController = controller.GetComponent<ExperienceController>();
        experienceController.OnSkillPointUsed += CheckIfPlayerHasEnoughSkillpoints;
        CheckIfPlayerHasEnoughSkillpoints();

        if (feat.StartingFeatLevel == 0) return;

        for (int i = 0; i < feat.StartingFeatLevel; i++)
        {
            ActivateButton(true);
        }
    }

    private void OnDisable()
    {
        experienceController.OnSkillPointUsed -= CheckIfPlayerHasEnoughSkillpoints;
    }

    public void CheckIfPlayerHasEnoughSkillpoints()
    {
        if (currentFeatLevel == feat.MaximumFeatLevel)
        {
            FeatName.color = DeactiveColor;
            return;
        }

        if (experienceController.SkillPoints >= feat.GetCostPerLevel(currentFeatLevel))
        {
            FeatName.color = ActiveColor;
        }
        else
        {
            FeatName.color = DeactiveColor;
        }
    }

    public void ActivateButton()
    {
        //controller.ActivateFeat(FeatIndex, FillNextBubble);
        controller.UnlockFeat(feat, FillNextBubble);
    }

    public void ActivateButton(bool bypassReqs = false)
    {
        //controller.ActivateFeat(FeatIndex, FillNextBubble, bypassReqs);
        controller.UnlockFeat(feat, FillNextBubble);
    }

    private void FillNextBubble()
    {
        currentFeatLevel++;
        CheckIfPlayerHasEnoughSkillpoints();
        playerFeatsPanelController.UpdateFeatPreviewWindow(currentFeatLevel, feat);

        for (int i = 0; i < currentFeatLevel; i++)
        {
            FeatBubbles[i].FillBubble();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        playerFeatsPanelController.UpdateFeatPreviewWindow(currentFeatLevel, feat);
        playerFeatsPanelController.UpdateViewPosition(rectTransform);
    }

    public void OnDeselect(BaseEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectable.Select();
    }
}
