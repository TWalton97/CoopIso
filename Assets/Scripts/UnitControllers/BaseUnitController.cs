using UnityEngine;

//Base unit controller that has functionality for a health controller component

[RequireComponent(typeof(HealthController))]
public class BaseUnitController : MonoBehaviour
{
    protected HealthController HealthController;
    public virtual void Awake()
    {
        HealthController = GetComponent<HealthController>();
    }

    public virtual void OnEnable()
    {
        HealthController.OnDie += Die;
    }

    public virtual void OnDisable()
    {
        HealthController.OnDie -= Die;
    }

    public virtual void TakeDamage(int damageAmount, BaseUnitController controller)
    {
        HealthController.TakeDamage(damageAmount, controller);
    }

    public virtual void Heal(int healAmount)
    {
        HealthController.Heal(healAmount);
    }

    public virtual void Die()
    {
        //Noop
    }
}
