using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsPanel : MonoBehaviour
{
    //We send a list of control data to this component
    //The component will build and enable controls as necessary
    public PlayerContext PlayerContext;
    public List<ControlPrompt> controlPrompts;

    public void UpdateControls(PlayerContext playerContext, List<ControlData> newControlData)
    {
        DisableAllControlPrompts();
        for (int i = 0; i < newControlData.Count; i++)
        {
            controlPrompts[i].SetPrompt(playerContext, newControlData[i]);
        }
    }

    public void DisableAllControlPrompts()
    {
        foreach (ControlPrompt controlPrompt in controlPrompts)
        {
            controlPrompt.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class ControlData
{
    public string InteractionText;
    public Sprite InteractionSpriteKBM;
    public Sprite InteractionSpriteGamepad;
}
