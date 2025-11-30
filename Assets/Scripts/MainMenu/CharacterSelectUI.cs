using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    public int PlayerIndex;
    public List<ClassButton> classButtons = new();
    public MainMenuController mainMenuController;

    public GameObject PressToJoinText;
    public GameObject SelectionButtons;

    public void EnableSelectionButtons()
    {
        PressToJoinText.SetActive(false);
        SelectionButtons.SetActive(true);
        classButtons[0].ToggleHighlight(true);
        classButtons[0].button.onClick.Invoke();
    }

    public void OnClassSelected(ClassPresetSO classPresetSO)
    {
        mainMenuController.gameSetupData.SelectPlayerClass(PlayerIndex, classPresetSO);

        foreach (ClassButton classButton in classButtons)
        {
            classButton.DisableSelectedIcon();
        }
    }

    public void ReturnToMainMenu()
    {
        mainMenuController.ShowMainMenu();
    }
}

