using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHitboxHandler : MonoBehaviour
{
    public List<HealthController> controllers = new();

    void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out HealthController controller))
        {
            if (controllers.Contains(controller)) return;

            controllers.Add(controller);
            controller.TakeDamage(5, null);
        }
    }
}
