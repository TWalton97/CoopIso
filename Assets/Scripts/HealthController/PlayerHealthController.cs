using UnityEngine;

public class PlayerHealthController : HealthController
{
    private NewPlayerController newPlayerController;
    public float blockAngle;

    private void Awake()
    {
        newPlayerController = GetComponent<NewPlayerController>();
    }

    public override void TakeDamage(int damageAmount, Entity controller, bool bypassBlockCheck = false)
    {
        if (!bypassBlockCheck)
        {
            if (newPlayerController.attackStateMachine.current.State == newPlayerController.blockState)
            {
                ShieldSO shieldData = newPlayerController.WeaponController.instantiatedSecondaryWeapon.Data as ShieldSO;
                if (CheckAngleToAttacker(controller.gameObject, shieldData.BlockAngle))
                {
                    Block(damageAmount, controller, shieldData);
                    return;
                }
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

    private bool CheckAngleToAttacker(GameObject attacker, float blockAngle)
    {
        var directionToPlayer = attacker.transform.position - transform.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);

        if (!(angleToPlayer < blockAngle / 2f))
        {
            return false;
        }
        return true;
    }


    //Calculates the new damage amount and returns it
    private void Block(int damageAmount, Entity controller, ShieldSO shieldData)
    {
        int newDamageAmount = Mathf.Clamp(damageAmount - shieldData.BlockAmount, 0, damageAmount);
        TakeDamage(newDamageAmount, controller, true);
    }
}
