using UnityEngine;

public class Feat : MonoBehaviour, IFeat
{
    public int CurrentFeatLevel = 0;
    public FeatSO FeatData;
    public virtual void OnActivate(FeatsController controller)
    {

    }
}
