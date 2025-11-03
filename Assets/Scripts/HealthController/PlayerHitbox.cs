using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    //Animation event calls function from this script when it wants to activate the hitbox
    //This script tells the weapon controller which weapon to activate

    //The weapon itself contains a hitbox and stores its own damage
    NewWeaponController WeaponController;

    void Awake()
    {
        WeaponController = GetComponentInParent<NewWeaponController>();
    }

    public void ActivatePrimaryWeaponHitbox()
    {
        if (WeaponController.instantiatedPrimaryWeapon != null)
            WeaponController.instantiatedPrimaryWeapon.ActivateHitbox();
    }

    public void ActivateSecondaryWeaponHitbox()
    {
        if (WeaponController.instantiatedSecondaryWeapon != null)
            WeaponController.instantiatedSecondaryWeapon.ActivateHitbox();
    }

}
