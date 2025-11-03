using UnityEngine;

public class AffixManager : Singleton<AffixManager>
{
    public GameObject commonVFX;
    public GameObject uncommonVFX;
    public GameObject rareVFX;
    public GameObject epicVFX;
    public GameObject legendaryVFX;

    public GameObject ReturnVFX(int numAffixes)
    {
        switch (numAffixes)
        {
            case 0:
                return null;
            case 1:
                return commonVFX;
            case 2:
                return uncommonVFX;
            case 3:
                return rareVFX;
            case 4:
                return epicVFX;
            case 5:
                return legendaryVFX;
        }
        return null;
    }
}
