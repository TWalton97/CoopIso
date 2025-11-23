using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshRendererBoneRef : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;

    public Transform GetRootBone()
    {
        return skinnedMeshRenderer.rootBone;
    }

    public Transform[] GetBones()
    {
        return skinnedMeshRenderer.bones;
    }
}
