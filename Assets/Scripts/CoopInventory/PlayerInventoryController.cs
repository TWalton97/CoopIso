using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour
{
    public List<GameObject> equipmentTabs;

    public void DisableAllTabs()
    {
        foreach (GameObject obj in equipmentTabs)
        {
            obj.SetActive(false);
        }
    }
}
