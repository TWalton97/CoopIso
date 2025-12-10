using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuNavigationController : MonoBehaviour
{
    //Controls navigation throughout the main menu
    public PlayerInputManager PlayerInputManager;
    public GameObject MainMenu;
    public GameObject CharacterSelectMenu;
    public LoadMenuManager LoadGameMenu;

    public UIButton[] MainMenuButtons;

    public PlaySessionData playSessionData;

    void Start()
    {
        playSessionData = PlaySessionData.Instance;
    }

    public void ShowCharacterSelectMenu()
    {
        playSessionData.PlaySessionLoadMode = GameLoadMode.NewGame;

        MainMenu.SetActive(false);
        CharacterSelectMenu.SetActive(true);
    }

    public void ShowLoadMenu()
    {
        playSessionData.PlaySessionLoadMode = GameLoadMode.LoadedGame;

        MainMenu.SetActive(false);
        LoadGameMenu.OpenLoadMenu();
    }

    public void ShowMainMenu()
    {
        DisableAllButtonHighlights();
        CharacterSelectMenu.SetActive(false);
        LoadGameMenu.CloseLoadMenu();
        MainMenu.SetActive(true);
    }

    private void DisableAllButtonHighlights()
    {
        foreach (UIButton button in MainMenuButtons)
        {
            button.ToggleHighlight(false);
        }
    }
}
