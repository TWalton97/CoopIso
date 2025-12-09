using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public PlayerInputManager PlayerInputManager;
    public GameObject MainMenu;
    public GameObject CharacterSelectMenu;
    public LoadMenuManager LoadGameMenu;

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
        MainMenu.SetActive(false);
        LoadGameMenu.gameObject.SetActive(true);
        LoadGameMenu.OpenLoadMenu();
    }

    public void LoadGame(GameStateData gameStateData)
    {
        gameLoadMode = GameLoadMode.LoadedGame;

        GameStateDataToLoad = gameStateData;

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

    public GameStateData LoadGameData(int slotIndex)
    {
        string slotFolder = Path.Combine(Application.persistentDataPath, "Saves", $"save{slotIndex}");
        string savePath = Path.Combine(slotFolder, "save.json");

        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"Save file not found for slot {slotIndex}: {savePath}");
            return null;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            GameStateData data = JsonUtility.FromJson<GameStateData>(json);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load save data for slot {slotIndex} at {savePath}\n{e}");
            return null;
        }
    }
}

public enum GameLoadMode
{
    NewGame,
    LoadedGame
}
