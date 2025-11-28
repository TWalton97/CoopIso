using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

[AddComponentMenu("UI/Inverted Mask", 14)]
public class InvertedMask : Mask
{
    public override Material GetModifiedMaterial(Material baseMaterial)
    {
        // Let Mask do its normal setup first
        var modifiedMaterial = base.GetModifiedMaterial(baseMaterial);

        // THEN just change stencil compare to "NotEqual"
        modifiedMaterial = new Material(modifiedMaterial);
        modifiedMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);

        return modifiedMaterial;
    }
}