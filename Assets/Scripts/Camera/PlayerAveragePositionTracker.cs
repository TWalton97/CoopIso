using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAveragePositionTracker : MonoBehaviour
{
    public float LeashRange = 10f;

    public List<NewPlayerController> playerObjects;

    public PlayerJoinManager playerJoinManager;

    public bool DisableLeashing = false;

    private void Start()
    {
        playerJoinManager = PlayerJoinManager.Instance;
        for (int i = 0; i < playerJoinManager.playerControllers.Count; i++)
        {
            playerObjects.Add(playerJoinManager.GetPlayerControllerByIndex(i));
        }
    }

    private void Update()
    {
        CalculateAveragePosition();
    }

    private void LateUpdate()
    {
        if (DisableLeashing) return;

        foreach (var p in playerObjects)
        {
            if (p.IsDead)
                return;

            Vector3 offset = p.transform.position - transform.position;
            float distance = offset.magnitude;

            if (distance > LeashRange)
            {
                // Bring player back to boundary
                Vector3 newPos = transform.position + offset.normalized * LeashRange;
                p.transform.position = newPos;
            }
        }
    }

    private void CalculateAveragePosition()
    {
        if (playerObjects.Count == 0) return;

        int numberOfPlayers = playerObjects.Count;
        Vector3 tempPos = Vector3.zero;

        foreach (NewPlayerController player in playerObjects)
        {
            if (player.IsDead)
            {
                numberOfPlayers--;
            }
            else
            {
                tempPos += player.transform.position;
            }
        }

        transform.position = tempPos / numberOfPlayers;
    }
}
