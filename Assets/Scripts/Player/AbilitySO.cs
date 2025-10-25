using UnityEngine;

public abstract class AbilitySO : ScriptableObject
{
    public float AbilityCost;
    public abstract void ActivateAbility(Transform transform, Vector3 attackDir);   //Projectile abilities
    public abstract void ActivateAbility(Transform transform);  //Aoe abilities
    public abstract void ActivateAbility(Vector3 pos, Transform transform);  //Ground targeted abilities
    public abstract bool CanActivate();
}
