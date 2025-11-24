using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public PlayerInputManager PlayerInputManager;
    public GameObject MainMenu;
    public GameObject CharacterSelectMenu;

    public GameSetupData gameSetupData;

    public string GameplaySceneName;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void OnNewGamePressed()
    {
        gameSetupData.Initialize(2);
        ShowCharacterSelectMenu();
        PlayerInputManager.DisableJoining();
    }

    public void ShowMainMenu()
    {
        CharacterSelectMenu.SetActive(false);
        MainMenu.SetActive(true);
        PlayerInputManager.EnableJoining();
    }

    public void ShowCharacterSelectMenu()
    {
        MainMenu.SetActive(false);
        CharacterSelectMenu.SetActive(true);
    }

    public void CheckIfPlayersAreReady()
    {
        if (!CheckReadyStatus()) return;

        gameSetupData.PlayerCount = PlayerInputManager.playerCount;
        SceneManager.LoadScene(GameplaySceneName);
    }

    private bool CheckReadyStatus()
    {
        for (int i = 0; i < PlayerInputManager.playerCount; i++)
        {
            if (!gameSetupData.Selections[i].isReady)
                return false;
        }
        return true;
    }
}
