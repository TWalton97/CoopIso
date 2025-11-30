using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : UIButton
{
    public CharacterSelectUI selectUI;

    public Image[] BackgroundImages;
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
            foreach (Image image in BackgroundImages)
            {
                image.color = ToggledColor;
            }
        }
        else
        {
            foreach (Image image in BackgroundImages)
            {
                image.color = BaseColor;
            }
        }
    }
}
