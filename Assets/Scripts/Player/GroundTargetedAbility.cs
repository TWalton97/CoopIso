using UnityEngine;

[CreateAssetMenu(fileName = "Abilities", menuName = "Abilities/GroundTargetedAbility")]
public class GroundTargetedAbility : BaseAbility
{
    public Hitbox hitbox;

    public override void ActivateAbility(Vector3 pos, Transform transform)
    {
        Hitbox _hitbox = Instantiate(hitbox, pos, Quaternion.identity);
        _hitbox.ActivateHitbox(10);
        Destroy(_hitbox.gameObject, 0.2f);
    }
}
