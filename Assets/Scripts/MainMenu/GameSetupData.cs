using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

[Serializable]
public class GameSetupData
{
    public int PlayerCount = 1;
    public PlayerSelectionData[] Selections = new PlayerSelectionData[2];
    public ClassPresetSO[] chosenClassPresets;

    public void Initialize(int numPlayers)
    {
        PlayerCount = numPlayers;
    }

    public void SelectPlayerClass(int playerIndex, ClassPresetSO preset)
    {
        chosenClassPresets[playerIndex] = preset;
    }

    public void ReadyUp(int playerIndex, bool isReady)
    {
        Selections[playerIndex].isReady = isReady;
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
