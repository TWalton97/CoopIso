using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    public int PlayerIndex;
    public List<ClassButton> classButtons = new();
    public MainMenuNavigationController mainMenuNavigationController;

    public GameObject PressToJoinText;
    public GameObject SelectionButtons;

    public PlaySessionData playSessionData;

    public void EnableSelectionButtons()
    {
        PressToJoinText.SetActive(false);
        SelectionButtons.SetActive(true);
        classButtons[0].ToggleHighlight(true);
        classButtons[0].button.onClick.Invoke();
    }

    public void OnClassSelected(ClassPresetSO classPresetSO)
    {
        playSessionData.gameSetupData.SelectPlayerClass(PlayerIndex, classPresetSO);

        foreach (ClassButton classButton in classButtons)
        {
            classButton.DisableSelectedIcon();
        }
    }

    public void ReturnToMainMenu()
    {
        mainMenuNavigationController.ShowMainMenu();
    }
}

