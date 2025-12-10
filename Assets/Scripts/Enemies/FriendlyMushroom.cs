using UnityEngine;

public class FriendlyMushroom : FriendlySkeletonWarrior
{
    public StatusHitbox PoisonPuffVFX;
    public GameObject PoisonPuffSpawnPosition;

    public override void Init(NewPlayerController controller)
    {
        base.Init(controller);
        float scale = Random.Range(0.9f, 1.1f);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public override void Die()
    {
        base.Die();
        Invoke(nameof(InstantiatePoisonPuffAtPosition), 2.5f);
    }

    public void InstantiatePoisonPuffAtPosition()
    {
        StatusHitbox obj = Instantiate(PoisonPuffVFX, PoisonPuffSpawnPosition.transform.position, Quaternion.identity);
        obj.Init(this);
        Destroy(obj.gameObject, 3f);
    }
}
