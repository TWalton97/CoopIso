using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    public int PlayerIndex;
    public List<ClassButton> classButtons = new();
    public MainMenuController mainMenuController;

    private void OnEnable()
    {
        classButtons[0].selectable.onClick.Invoke();
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

