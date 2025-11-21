using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FeatButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    public Button button;
    public TMP_Text FeatName;
    public List<LevelBubble> FeatBubbles;
    private FeatsController controller;
    private ExperienceController experienceController;
    private PlayerFeatsPanelController playerFeatsPanelController;
    public int FeatIndex;
    public Button selectable;
    public Feat feat { get; private set; }
    public Color DeactiveColor;
    public Color ActiveColor;

    public void InitializeButton(Feat _feat, FeatsController _controller, PlayerFeatsPanelController _playerFeatsPanelController, int _featIndex)
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
        if (experienceController.SkillPoints >= feat.SkillPointsCostPerLevel)
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
        controller.ActivateFeat(FeatIndex, FillNextBubble);
    }

    public void ActivateButton(bool bypassReqs = false)
    {
        controller.ActivateFeat(FeatIndex, FillNextBubble, bypassReqs);
    }

    private void FillNextBubble()
    {
        CheckIfPlayerHasEnoughSkillpoints();
        playerFeatsPanelController.UpdateFeatPreviewWindow(feat);

        for (int i = 0; i < controller.AvailableFeats[FeatIndex].CurrentFeatLevel; i++)
        {
            FeatBubbles[i].FillBubble();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        playerFeatsPanelController.UpdateFeatPreviewWindow(feat);
    }

    public void OnDeselect(BaseEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectable.Select();
    }
}
