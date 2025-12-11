using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;

public class SaveButton : UIButton
{
    public TMP_Text ZoneNameText;
    public TMP_Text SaveDateText;
    public TMP_Text SaveTimeText;

    public TMP_Text PlayTimeText;
    public TMP_Text DateStartedText;
    public TMP_Text NumberOfPlayersText;

    public string ZoneName;
    public long SaveDate;
    public int PlayTime;
    public long DateStarted;
    public List<string> Players;

    public SaveSlotMetaData saveSlotMetaData;

    public int CheckpointIndex;

    void OnEnable()
    {
        UpdateButtonInfo();
    }

    public void UpdateButtonInfo()
    {
        if (ZoneName == "")
        {
            ZoneNameText.text = "Empty Slot";
        }
        else
        {
            ZoneNameText.text = ZoneName;
        }

        if (SaveDate == 0)
        {
            SaveDateText.text = "-";
            SaveTimeText.text = "-";
        }
        else
        {
            DateTime saveTime = new DateTime(SaveDate, DateTimeKind.Utc).ToLocalTime();
            SaveDateText.text = saveTime.ToString("yyyy-MM-dd");
            SaveTimeText.text = saveTime.ToString("h:mm tt");
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        ToggleHighlight(true);
        UpdatePreview();
    }

    public void UpdatePreview()
    {
        float totalSeconds = PlayTime;
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60);
        PlayTimeText.text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        if (DateStarted == 0)
        {
            DateStartedText.text = "-";
        }
        else
        {
            DateTime saveTime = new DateTime(DateStarted, DateTimeKind.Utc).ToLocalTime();
            DateStartedText.text = saveTime.ToString("yyyy-MM-dd h:mm tt");
        }
        NumberOfPlayersText.text = FormatClasses(Players);
    }


    public override void OnDeselect(BaseEventData eventData)
    {
        ToggleHighlight(false);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        ToggleHighlight(true);
        button.Select();
    }

    public void SaveGameToFile(int slotIndex)
    {
        //SaveGame.Instance.Save(slotIndex);

        ZoneManager.Instance.LastCheckpointIndex = CheckpointIndex;
        SaveManager.Instance.SaveGame(slotIndex);
        SaveMenuManager.Instance.CloseSaveMenu();
    }

    public void LoadGameFromFile(int slotIndex)
    {
        if (saveSlotMetaData == null) return;

        Debug.Log($"Loading game from save slot {slotIndex}");
        LoadManager.Instance.LoadGame(slotIndex);
        PlaySessionData.Instance.StartLoadedGame();
    }

    string FormatClasses(List<string> classes)
    {
        if (classes == null || classes.Count == 0)
            return "0 (-)";

        return $"{classes.Count} ({string.Join(", ", classes)})";
    }
}
