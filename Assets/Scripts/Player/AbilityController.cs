using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public AbilitySO ability;

    public void CallAbility(Vector3 pos, Transform transform)
    {
        ability.ActivateAbility(pos, transform);
    }

}
