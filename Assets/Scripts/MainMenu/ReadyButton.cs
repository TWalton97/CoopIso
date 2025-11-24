using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    public CharacterSelectUI selectUI;

    public Image image;
    public Color BaseColor;
    public Color ToggledColor;

    public bool ToggledOn = false;

    public void ToggleButton()
    {
        ToggledOn = !ToggledOn;
        selectUI.mainMenuController.gameSetupData.ReadyUp(selectUI.PlayerIndex, ToggledOn);
        selectUI.mainMenuController.CheckIfPlayersAreReady();

        if (ToggledOn)
        {
            image.color = ToggledColor;
        }
        else
        {
            image.color = BaseColor;
        }
    }
}
