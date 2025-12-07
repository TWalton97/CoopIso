using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GlossaryButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    [System.Serializable]
    public class GlossaryEntry
    {
        public string Name;
        [TextArea]
        public string Description;
    }
    public Button selectable;
    public GlossaryController GlossaryController;
    public GlossaryEntry glossaryEntry;

    public void OnSelect(BaseEventData eventData)
    {
        GlossaryController.FillGlossaryEntry(glossaryEntry.Name, glossaryEntry.Description);
    }

    public void OnDeselect(BaseEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerId != GlossaryController.PlayerContext.PlayerIndex) return;
        selectable.Select();
    }
}
