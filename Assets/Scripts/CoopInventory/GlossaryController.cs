using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GlossaryController : MonoBehaviour
{
    public PlayerContext PlayerContext;
    public TMP_Text GlossaryEntryName;
    public TMP_Text GlossaryEntryDescription;
    public Button glossaryButtonOne;

    public List<ControlData> ControlData;

    void OnEnable()
    {
        PlayerContext.UserInterfaceController.inventoryController.UpdateControlPanel(ControlData);
    }

    public void FillGlossaryEntry(string name, string description)
    {
        GlossaryEntryName.text = name;
        GlossaryEntryDescription.text = description;
    }
}
