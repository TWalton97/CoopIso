using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAveragePositionTracker : MonoBehaviour
{
    public List<GameObject> playerObjects;

    private void Update()
    {
        CalculateAveragePosition();
    }

    public void AddPlayer(GameObject obj)
    {
        playerObjects.Add(obj);
    }

    private void CalculateAveragePosition()
    {
        if (playerObjects.Count == 0) return;

        Vector3 tempPos = Vector3.zero;

        foreach (GameObject player in playerObjects)
        {
            tempPos += player.transform.position;
        }

        transform.position = tempPos / playerObjects.Count;
    }
}
