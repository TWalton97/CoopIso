using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class FeatButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image TabImage;
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

    public Color NormalButtonColor, HoveredButtonColor;

    public int currentFeatLevel;

    public bool IsSelected;

    public void InitializeButton(FeatSO _feat, FeatsController _controller, PlayerFeatsPanelController _playerFeatsPanelController, int _featIndex)
    {
        FeatIndex = _featIndex;
        controller = _controller;
        playerFeatsPanelController = _playerFeatsPanelController;
        feat = _feat;
        FeatName.text = feat.FeatName;
        button.onClick.AddListener(ActivateButton);
        controller.OnFeatLevelChanged += UpdateUI;

        for (int i = 0; i < feat.SkillPointCostPerLevel.Length; i++)
        {
            FeatBubbles[i].gameObject.SetActive(true);
        }

        experienceController = controller.GetComponent<ExperienceController>();
        experienceController.OnSkillPointUsed += CheckIfPlayerHasEnoughSkillpoints;
        CheckIfPlayerHasEnoughSkillpoints();

        UpdateUI(_feat, _controller.GetCurrentFeatLevel(_feat));
    }

    private void OnDisable()
    {
        experienceController.OnSkillPointUsed -= CheckIfPlayerHasEnoughSkillpoints;
    }

    void OnDestroy()
    {
        controller.OnFeatLevelChanged -= UpdateUI;
    }

    public void CheckIfPlayerHasEnoughSkillpoints()
    {
        if (currentFeatLevel == feat.SkillPointCostPerLevel.Length)
        {
            FeatName.color = DeactiveColor;
            return;
        }

        if (experienceController.SkillPoints >= feat.SkillPointCostPerLevel[Mathf.Clamp(currentFeatLevel - 1, 0, feat.SkillPointCostPerLevel.Length)])
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
        controller.UnlockFeat(feat);
    }

    public void ActivateButton(bool bypassReqs = false)
    {
        controller.UnlockFeat(feat);
    }

    private void UpdateUI(FeatSO changedFeat, int newLevel)
    {
        if (changedFeat != feat) return;

        currentFeatLevel = newLevel;
        playerFeatsPanelController.UpdateFeatPreviewWindow(currentFeatLevel, feat);
        CheckIfPlayerHasEnoughSkillpoints();

        for (int i = 0; i < currentFeatLevel; i++)
        {
            FeatBubbles[i].FillBubble();
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        IsSelected = true;
        playerFeatsPanelController.UpdateFeatPreviewWindow(currentFeatLevel, feat);
        playerFeatsPanelController.UpdateViewPosition(rectTransform);
        ToggleHighlight(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ToggleHighlight(false);
        IsSelected = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playerFeatsPanelController.PlayerContext.PlayerInput.currentControlScheme != playerFeatsPanelController.PlayerContext.PlayerController.KEYBOARD_SCHEME)
            return;

        ToggleHighlight(true);
        IsSelected = true;
        playerFeatsPanelController.UpdateFeatPreviewWindow(currentFeatLevel, feat);
        playerFeatsPanelController.UpdateViewPosition(rectTransform);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (playerFeatsPanelController.PlayerContext.PlayerInput.currentControlScheme != playerFeatsPanelController.PlayerContext.PlayerController.KEYBOARD_SCHEME)
            return;

        ToggleHighlight(false);
        IsSelected = false;
    }

    public virtual void ToggleHighlight(bool toggle)
    {
        IsSelected = toggle;
        if (toggle)
        {
            TabImage.color = HoveredButtonColor;
        }
        else
        {
            TabImage.color = NormalButtonColor;
        }
    }
}
