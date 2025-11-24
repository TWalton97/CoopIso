using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GlossaryController : MonoBehaviour
{
    public TMP_Text GlossaryEntryName;
    public TMP_Text GlossaryEntryDescription;
    public Button glossaryButtonOne;

    public void FillGlossaryEntry(string name, string description)
    {
        GlossaryEntryName.text = name;
        GlossaryEntryDescription.text = description;
    }
}
