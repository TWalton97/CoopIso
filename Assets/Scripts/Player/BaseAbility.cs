using UnityEngine;

public class BaseAbility : AbilitySO
{
    public override void ActivateAbility(Transform transform, Vector3 attackDir)
    {

    }

    public override void ActivateAbility(Transform transform)
    {

    }

    public override void ActivateAbility(Vector3 pos, Transform transform)
    {

    }

    public override bool CanActivate()
    {
        return true;
    }
}
