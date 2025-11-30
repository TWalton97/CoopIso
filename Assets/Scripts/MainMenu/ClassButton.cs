using UnityEngine;
using UnityEngine.EventSystems;

public class ClassButton : UIButton, IPointerEnterHandler
{
    public GameObject SelectedIcon;

    public void DisableSelectedIcon()
    {
        SelectedIcon.SetActive(false);
    }

    public void EnableSelectedIcon()
    {
        SelectedIcon.SetActive(true);
    }
}

