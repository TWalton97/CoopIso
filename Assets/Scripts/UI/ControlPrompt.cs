using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlPrompt : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text label;

    public void SetPrompt(PlayerContext playerContext, ControlData controlData)
    {
        if (playerContext.PlayerInput.currentControlScheme == playerContext.PlayerController.GAMEPAD_SCHEME)
        {
            iconImage.sprite = controlData.InteractionSpriteGamepad;
        }
        else
        {
            iconImage.sprite = controlData.InteractionSpriteKBM;
        }
        label.text = controlData.InteractionText;
        gameObject.SetActive(true);
    }
}
