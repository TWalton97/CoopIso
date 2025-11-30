using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPlayerController : MonoBehaviour
{
    public SkinnedMeshRendererBoneRef skinnedMeshRendererBoneRef;

    public Transform MainHandTransform;
    public Transform OffHandTransform;

    public Transform HelmetTransform;
    public Transform BodyTransform;
    public Transform LegsTransform;

    private GameObject instantiatedMainHand;
    private GameObject instantiatedOffHand;

    private GameObject instantiatedHelmet;
    private GameObject instantiatedBody;
    private GameObject instantiatedLegs;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    public void EquipArmorToSlot(ItemType itemType, GameObject prefab)
    {
        switch (itemType)
        {
            case ItemType.Head:
                if (instantiatedHelmet != null)
                {
                    Destroy(instantiatedHelmet);
                }
                instantiatedHelmet = Instantiate(prefab, HelmetTransform.position, Quaternion.identity, HelmetTransform);
                skinnedMeshRenderer = instantiatedHelmet.GetComponent<SkinnedMeshRenderer>();
                skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
                skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
                break;
            case ItemType.Body:
                if (instantiatedBody != null)
                {
                    Destroy(instantiatedBody);
                }
                instantiatedBody = Instantiate(prefab, BodyTransform.position, Quaternion.identity, BodyTransform);
                skinnedMeshRenderer = instantiatedBody.GetComponent<SkinnedMeshRenderer>();
                skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
                skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
                break;
            case ItemType.Legs:
                if (instantiatedLegs != null)
                {
                    Destroy(instantiatedLegs);
                }
                instantiatedLegs = Instantiate(prefab, LegsTransform.position, Quaternion.identity, LegsTransform);
                skinnedMeshRenderer = instantiatedLegs.GetComponent<SkinnedMeshRenderer>();
                skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
                skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
                break;
        }
    }

    public void UnequipArmor(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Head:
                if (instantiatedHelmet != null)
                {
                    Destroy(instantiatedHelmet);
                }
                break;
            case ItemType.Body:
                if (instantiatedBody != null)
                {
                    Destroy(instantiatedBody);
                }
                break;
            case ItemType.Legs:
                if (instantiatedLegs != null)
                {
                    Destroy(instantiatedLegs);
                }
                break;
        }
    }

    public void EquipWeaponToSlot(Weapon.WeaponHand weaponHand, GameObject prefab)
    {
        switch (weaponHand)
        {
            case Weapon.WeaponHand.MainHand:
                if (instantiatedMainHand != null)
                {
                    Destroy(instantiatedMainHand);
                }
                if (prefab != null)
                {
                    instantiatedMainHand = Instantiate(prefab, MainHandTransform.position, Quaternion.identity, MainHandTransform);
                    instantiatedMainHand.transform.localRotation = prefab.transform.rotation;
                }
                break;
            case Weapon.WeaponHand.OffHand:
                if (instantiatedOffHand != null)
                {
                    Destroy(instantiatedOffHand);
                }
                if (prefab != null)
                {
                    instantiatedOffHand = Instantiate(prefab, OffHandTransform.position, Quaternion.identity, OffHandTransform);
                    instantiatedOffHand.transform.localRotation = prefab.transform.rotation;
                }
                break;
        }
    }

    public void UnequipWeapon(Weapon.WeaponHand weaponHand)
    {
        switch (weaponHand)
        {
            case Weapon.WeaponHand.MainHand:
                if (instantiatedMainHand != null)
                {
                    Destroy(instantiatedMainHand);
                }
                break;
            case Weapon.WeaponHand.OffHand:
                if (instantiatedOffHand != null)
                {
                    Destroy(instantiatedOffHand);
                }
                break;
        }
    }
}
