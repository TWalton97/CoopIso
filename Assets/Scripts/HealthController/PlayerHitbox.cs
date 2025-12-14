using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    NewWeaponController WeaponController;

    void Awake()
    {
        WeaponController = GetComponentInParent<NewWeaponController>();
    }

    public void ActivatePrimaryWeaponHitbox()
    {
        if (WeaponController.instantiatedPrimaryWeapon != null)
            WeaponController.instantiatedPrimaryWeapon.ActivateHitbox(WeaponController.numAttacks);
    }

    public void ActivateSecondaryWeaponHitbox()
    {
        if (WeaponController.instantiatedSecondaryWeapon != null)
            WeaponController.instantiatedSecondaryWeapon.ActivateHitbox(WeaponController.numAttacks);
    }
}
