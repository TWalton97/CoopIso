
using UnityEngine;
public class UtilityMathFunctions
{
    public Vector3 ConvertGlobalDirectionToLocal(Transform transform, Vector3 direction)
    {
        var dir = transform.TransformDirection(direction);
        return dir;
    }
}
