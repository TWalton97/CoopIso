using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static LayerMask GROUND_LAYER = 1 >> 6;
    public static Vector3 ReturnRandomPointOnXZ(Vector3 sourcePosition, float radius)
    {
        Vector3 samplePosition = Random.onUnitSphere;
        return sourcePosition + (new Vector3(samplePosition.x, 1f, samplePosition.y) * radius);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask);
        return navHit.position;
    }
}
