using System.Collections;
using Cinemachine.Utility;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/Self Aura")]
public class SelfAuraSO : StatusSO
{
    public int Damage;
    public float TickRate = 0.5f;
    public LayerMask TargetLayer;


    public DamageOverTimeHitbox AuraGameObject;
    private DamageOverTimeHitbox instantiatedAuraGameObject;

    public override void OnEnter(StatusInstance instance, StatusController target)
    {
        base.OnEnter(instance, target);
        if (instantiatedAuraGameObject == null)
        {
            instantiatedAuraGameObject = GameObject.Instantiate(AuraGameObject, target.transform.position, target.transform.rotation);
            instantiatedAuraGameObject.Init(Damage, TargetLayer, target.GetComponent<Entity>(), false, TickRate, true);
        }
    }

    public override void OnTick(StatusInstance instance, StatusController target, float deltaTime)
    {
        if (instantiatedAuraGameObject != null)
        {
            instantiatedAuraGameObject.transform.position = target.transform.position;
        }
    }

    public override void OnExit(StatusInstance instance, StatusController target)
    {
        if (instantiatedAuraGameObject != null)
        {
            instantiatedAuraGameObject.enabled = false;
            ParticleSystem ps = instantiatedAuraGameObject.GetComponentInChildren<ParticleSystem>();
            ps.Stop();
            target.StartCoroutine(WaitForParticleSystemToBeDestroyed(ps));
        }
    }

    private IEnumerator WaitForParticleSystemToBeDestroyed(ParticleSystem particleSystem)
    {
        while (particleSystem != null)
            yield return null;

        Destroy(instantiatedAuraGameObject.gameObject);
        instantiatedAuraGameObject = null;
    }
}
