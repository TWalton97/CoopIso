using UnityEngine;

[CreateAssetMenu(menuName = "Gems/Effects/Stat Modifier")]
public class GemStatEffectSO : GemEffectSO
{
    public override void Apply(GemContext context)
    {
        //Apply the stat here
    }

    public override void Deregister()
    {

    }

    public override void ApplyOnce(GemContext context, Entity target)
    {

    }
}


