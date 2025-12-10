using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

[Serializable]
public class GameSetupData
{
    public int PlayerCount = 2;
    public PlayerSelectionData[] Selections = new PlayerSelectionData[2];
    public ClassPresetSO[] chosenClassPresets = new ClassPresetSO[2];

    public void SelectPlayerClass(int playerIndex, ClassPresetSO preset)
    {
        chosenClassPresets[playerIndex] = preset;
    }

    public void ReadyUp(int playerIndex, bool isReady)
    {
        Selections[playerIndex].isReady = isReady;
        PlaySessionData.Instance.CheckIfPlayersAreReady();
    }

    public void LockInPlayerCount()
    {
        PlayerCount = 0;
        for (int i = 0; i < 2; i++)
        {
            if (chosenClassPresets[i] != null)
            {
                PlayerCount++;
            }
        }
    }
}

[System.Serializable]
public class PlayerSelectionData
{
    public int characterID = -1;
    public bool isReady = false;
    public InputDevice PlayerDevices;
    public string PlayerControlSchemes;
}
