using Unity.VisualScripting;
using UnityEngine;

public class Movement : WeaponComponent
{
    private MovementData data;

    private void HandleStartMovement()
    {
        var currentAttackData = data.attackData[weapon.CurrentAttackCounter];

        playerController.SetVelocity(currentAttackData.Direction, currentAttackData.Velocity);
    }

    private void HandleStopMovement()
    {
        playerController.Rigidbody.velocity = Vector3.zero;
    }

    protected override void Awake()
    {
        base.Awake();

        data = weapon.Data.GetData<MovementData>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        EventHandler.OnStartMovement += HandleStartMovement;
        EventHandler.OnStopMovement += HandleStopMovement;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        EventHandler.OnStartMovement -= HandleStartMovement;
        EventHandler.OnStopMovement -= HandleStopMovement;
    }
}
