using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySessionData : Singleton<PlaySessionData>
{
    //Player selections
    public GameSetupData gameSetupData;             //Stores player selections, devices, and ready status

    public GameStateData PlaySessionGameData;       //Curent game data
    public SaveSlotMetaData PlaySessionMetaData;    //Current meta data
    public GameLoadMode PlaySessionLoadMode;        //New Game or Load Game

    public void CheckIfPlayersAreReady()
    {
        if (!CheckReadyStatus()) return;

        gameSetupData.LockInPlayerCount();
        PlaySessionGameData = LoadManager.Instance.ReturnGameStateData(-1);
        PlaySessionMetaData = LoadManager.Instance.ReturnSaveSlotMetaData(-1);
        StartLoadedGame();
    }

    private bool CheckReadyStatus()
    {
        int playerCount = 0;
        foreach (ClassPresetSO classPresetSO in gameSetupData.chosenClassPresets)
        {
            if (classPresetSO != null)
                playerCount++;
        }

        for (int i = 0; i < playerCount; i++)
        {
            if (!gameSetupData.Selections[i].isReady)
                return false;
        }
        return true;
    }

    public void StartLoadedGame()
    {
        SceneLoadingManager.Instance.LoadSceneGroup(SceneGroupDatabase.GetSceneGroup(PlaySessionGameData.LastCheckpointSaveData.sceneGroup), true, true);
    }
}
