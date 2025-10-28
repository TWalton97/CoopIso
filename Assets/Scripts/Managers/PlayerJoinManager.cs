using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerJoinManager : Singleton<PlayerJoinManager>
{
    public PlayerInputManager playerInputManager;

    public static Action<GameObject> OnPlayerJoinedEvent;
    public static Action<GameObject> OnPlayerLeftEvent;

    protected override void Awake()
    {
        base.Awake();

        playerInputManager = GetComponent<PlayerInputManager>();
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player joined");
        OnPlayerJoinedEvent?.Invoke(playerInput.gameObject);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        OnPlayerLeftEvent?.Invoke(playerInput.gameObject);
    }

}
