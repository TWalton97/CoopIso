using UnityEngine;

public class PlayerHealthController : HealthController
{
    private NewPlayerController newPlayerController;
    public float blockAngle;

    private void Awake()
    {
        newPlayerController = GetComponent<NewPlayerController>();
    }

    public override void TakeDamage(int damageAmount, Entity controller)
    {

        if (newPlayerController.attackStateMachine.current.State == newPlayerController.blockState)
        {
            if (CheckAngleToAttacker(controller.gameObject))
            {
                Block();
                return;
            }
        }
        if (IsDead) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - damageAmount, 0, MaximumHealth);
        OnTakeDamage?.Invoke(damageAmount, controller);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private bool CheckAngleToAttacker(GameObject attacker)
    {
        var directionToPlayer = attacker.transform.position - transform.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);

        if (!(angleToPlayer < blockAngle / 2f))
        {
            return false;
        }
        return true;
    }

    private void Block()
    {
        //Can do block stuff here if we want
    }
}
