using System.IO;
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

    public GameStateData GameStateDataToLoad;

    public GameLoadMode gameLoadMode;
    public SceneGroup SceneGroupToLoad;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void OnNewGamePressed()
    {
        gameLoadMode = GameLoadMode.NewGame;
        gameSetupData.Initialize(2);
        ShowCharacterSelectMenu();
    }

    public void OnLoadGamePressed()
    {
        gameLoadMode = GameLoadMode.LoadedGame;

        string path = Path.Combine(Application.persistentDataPath, "save1.json");
        if (!File.Exists(path)) return;
        string json = File.ReadAllText(path);
        GameStateDataToLoad = JsonUtility.FromJson<GameStateData>(json);

        SceneLoadingManager.Instance.LoadSceneGroup(SceneGroupDatabase.GetSceneGroup(GameStateDataToLoad.LastCheckpointSaveData.sceneGroup), true);
    }

    public void ShowMainMenu()
    {
        CharacterSelectMenu.SetActive(false);
        MainMenu.SetActive(true);
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
        SceneLoadingManager.Instance.LoadSceneGroup(SceneGroupToLoad);
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

public enum GameLoadMode
{
    NewGame,
    LoadedGame
}
