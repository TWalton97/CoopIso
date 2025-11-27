using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAveragePositionTracker : MonoBehaviour
{
    public List<GameObject> playerObjects;

    public PlayerJoinManager playerJoinManager;

    private void Start()
    {
        playerJoinManager = PlayerJoinManager.Instance;
        for (int i = 0; i < playerJoinManager.playerControllers.Count; i++)
        {
            playerObjects.Add(playerJoinManager.GetPlayerControllerByIndex(i).gameObject);
        }
    }

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
