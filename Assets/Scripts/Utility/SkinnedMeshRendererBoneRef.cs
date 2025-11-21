using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshRendererBoneRef : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    public Transform RootBone;
    public Transform[] Bones;

    void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        CopyBones();
    }

    private void CopyBones()
    {
        RootBone = skinnedMeshRenderer.rootBone;
        Bones = skinnedMeshRenderer.bones;
    }

}
