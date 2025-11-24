using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClassButton : MonoBehaviour, IPointerEnterHandler
{
    public CharacterSelectUI characterSelectUI;
    public int playerIndex;

    public Button selectable;
    public GameObject SelectedIcon;

    void Awake()
    {
        selectable = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selectable.Select();
    }

    public void DisableSelectedIcon()
    {
        SelectedIcon.SetActive(false);
    }

    public void EnableSelectedIcon()
    {
        SelectedIcon.SetActive(true);
    }
}

