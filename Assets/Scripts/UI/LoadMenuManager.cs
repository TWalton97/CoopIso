using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
public class LoadMenuManager : MonoBehaviour
{
    public GameObject MenuPanel;

    public GameObject LoadFirstSelected;
    public SaveButton[] SaveButtons;

    public EventSystem eventSystem;

    public void OpenLoadMenu()
    {
        gameObject.SetActive(true);
        LoadSaveSlotUI();

        eventSystem.SetSelectedGameObject(SaveButtons[0].gameObject);
        SaveButtons[0].UpdatePreview();
        foreach (SaveButton button in SaveButtons)
        {
            button.UpdateButtonInfo();
        }
    }

    public void CloseLoadMenu()
    {
        gameObject.SetActive(false);

        MenuPanel.SetActive(false);

        foreach (SaveButton button in SaveButtons)
        {
            button.ToggleHighlight(false);
        }
    }

    private void LoadSaveSlotUI()
    {
        List<SaveSlotMetaData> metadata = LoadAllMetaData();

        for (int i = 0; i < metadata.Count; i++)
        {
            SaveButtons[i].saveSlotMetaData = metadata[i];
            SaveButtons[i].ZoneName = metadata[i].LastZoneName;
            SaveButtons[i].SaveDate = metadata[i].LastSavedTimestamp;
            SaveButtons[i].PlayTime = metadata[i].TotalSessionPlaytimeSeconds;
            SaveButtons[i].DateStarted = metadata[i].SaveCreatedTimestamp;
            List<string> players = new();
            foreach (string c in metadata[i].playerClasses)
            {
                players.Add(c);
            }
            SaveButtons[i].Players = players;
        }
    }

    public List<SaveSlotMetaData> LoadAllMetaData()
    {
        string folder = Path.Combine(Application.persistentDataPath, "Saves");

        if (!Directory.Exists(folder))
        {
            return new List<SaveSlotMetaData>();
        }

        List<SaveSlotMetaData> metadataList = new List<SaveSlotMetaData>();

        string[] metaFiles = Directory.GetFiles(folder, "metadata.json", SearchOption.AllDirectories);

        foreach (string metaPath in metaFiles)
        {
            try
            {
                string json = File.ReadAllText(metaPath);
                SaveSlotMetaData meta = JsonUtility.FromJson<SaveSlotMetaData>(json);
                metadataList.Add(meta);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load metadata at: {metaPath}\n{e}");
            }
        }

        return metadataList;
    }

    public void LoadMainMenu()
    {
        SceneLoadingManager.Instance.LoadMainMenu();
    }
}
