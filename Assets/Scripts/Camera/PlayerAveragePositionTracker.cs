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

    private void Start()
    {
        PlayerJoinManager.OnPlayerJoinedEvent += AddPlayer;
    }
    void OnDisable()
    {
        PlayerJoinManager.OnPlayerJoinedEvent -= AddPlayer;
    }

    private void AddPlayer(GameObject obj)
    {
        Debug.Log("Adding player");
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
