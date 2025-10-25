using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    public float UPDATE_TICK_DURATION = 0.5f;
    public float AGGRO_RANGE;
    public float LEASH_RANGE;

    public LayerMask TargetLayerMask;

    public bool DisplayGizmo = false;

    [ReadOnly] public List<Collider> invalidTargets = new();    //We store of a list of invalid targets for easier iterating later
    [ReadOnly] public List<BaseUnitController> validTargets = new();

    [ReadOnly] public float _elapsedTime = 0f;


    public BaseUnitController ReturnValidBaseUnitController()
    {
        _elapsedTime -= Time.deltaTime;

        if (_elapsedTime > 0f)
        {
            CleanNullsFromList(validTargets);
            if (validTargets.Count == 0) return null;
            return validTargets[0];
        }

        Collider[] overlappingColls = Physics.OverlapSphere(transform.position, AGGRO_RANGE, TargetLayerMask);
        if (overlappingColls.Length == 0) return null;

        foreach (Collider coll in overlappingColls)
        {
            if (invalidTargets.Contains(coll)) continue;

            if (coll.TryGetComponent(out BaseUnitController controller))
            {
                validTargets.Add(controller);
            }
            else
            {
                invalidTargets.Add(coll);
            }
        }

        if (validTargets.Count == 0) return null;


        _elapsedTime = UPDATE_TICK_DURATION;

        return validTargets[0];
    }

    public bool IsTargetStillInRange(BaseUnitController baseUnitController)
    {
        if (Vector3.Distance(transform.position, baseUnitController.transform.position) < LEASH_RANGE)
        {
            return true;
        }
        return false;
    }

    private void CleanNullsFromList(List<BaseUnitController> baseUnitControllers)
    {
        baseUnitControllers.RemoveAll(item => item == null);
    }

    public void OnDrawGizmos()
    {
        if (DisplayGizmo == false) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AGGRO_RANGE);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, LEASH_RANGE);
    }
}



