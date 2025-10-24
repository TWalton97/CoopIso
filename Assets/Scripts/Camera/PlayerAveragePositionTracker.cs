using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAveragePositionTracker : MonoBehaviour
{
    public List<GameObject> playerObjects;

    private void Update()
    {
        CalculateAveragePosition();
    }

    private void CalculateAveragePosition()
    {
        Vector3 tempPos = Vector3.zero;

        foreach (GameObject player in playerObjects)
        {
            tempPos += player.transform.position;
        }

        transform.position = tempPos / playerObjects.Count;
    }
}
