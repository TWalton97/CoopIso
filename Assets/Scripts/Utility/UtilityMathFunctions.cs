
using UnityEngine;
public static class UtilityMathFunctions
{
    public static Vector3 ConvertGlobalDirectionToLocal(Transform transform, Vector3 direction)
    {
        var dir = transform.TransformDirection(direction);
        return dir;
    }

    public static float SquaredDistance(Vector3 startPoint, Vector3 endPoint)
    {
        Vector3 offSet = endPoint - startPoint;
        float sqrLen = offSet.sqrMagnitude;

        return sqrLen;
    }
}
