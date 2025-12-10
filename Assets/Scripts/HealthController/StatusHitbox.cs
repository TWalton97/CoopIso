using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHitbox : MonoBehaviour
{
    public Entity entity;
    public StatusSO StatusToApply;
    public List<StatusController> StatusControllers = new();

    public void Init(Entity entity)
    {
        this.entity = entity;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out StatusController statusController))
        {
            if (StatusControllers.Contains(statusController)) return;
            StatusControllers.Add(statusController);

            statusController.ApplyStatus(StatusToApply, entity);
        }
    }
}
