using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBubble : MonoBehaviour
{
    public GameObject BubbleFillGameObject;

    public void FillBubble()
    {
        BubbleFillGameObject.SetActive(true);
    }

    public void EmptyBubble()
    {
        BubbleFillGameObject.SetActive(false);
    }
}
