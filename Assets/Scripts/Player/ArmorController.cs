using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorController : MonoBehaviour
{
    //Stores a reference to each transform
    //Handles instantiating prefabs in their correct position and storing data
    public NewPlayerController PlayerController;
    public SkinnedMeshRendererBoneRef skinnedMeshRendererBoneRef;

    [Header("Body Armor")]
    public Transform bodyArmorTransform;
    public EquippedArmor instantiatedBodyArmor;

    [Header("Helmet")]
    public Transform helmetTransform;
    public EquippedArmor instantiatedHelmet;

    [Header("Legs")]
    public Transform legsTransform;
    public EquippedArmor instantiatedLegs;

    private bool StarterItemsEquipped = false;

    public Action<string> OnArmorUnequipped;

    public class EquippedArmor
    {
        public GameObject instantiatedObject;
        public ItemData itemData;

        public EquippedArmor(GameObject _instantiatedObject, ItemData _itemData)
        {
            instantiatedObject = _instantiatedObject;
            itemData = _itemData;
        }
    }

    public void EquipArmor(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemType.Head:
                EquipHelmet(itemData);
                break;
            case ItemType.Body:
                EquipBodyArmor(itemData);
                break;
            case ItemType.Legs:
                EquipLegs(itemData);
                break;
        }
    }

    public void UnequipArmor(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemType.Head:
                UnequipHelmet();
                break;
            case ItemType.Body:
                UnequipBodyArmor();
                break;
            case ItemType.Legs:
                UnequipLegs();
                break;
        }
    }

    public void EquipBodyArmor(ItemData itemData)
    {
        if (instantiatedBodyArmor == null)
        {
            GameObject obj = Instantiate(itemData.objectPrefab, bodyArmorTransform.position, Quaternion.identity, bodyArmorTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
            instantiatedBodyArmor = new EquippedArmor(obj, itemData);
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            PlayerController.HealthController.UpdateArmorAmount(spawnedArmorData.armorAmount);
            PlayerController.PlayerContext.PlayerPreviewManager.EquipArmorToPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Body, itemData.objectPrefab);
        }
        else
        {
            UnequipBodyArmor();
            EquipHelmet(itemData);
        }
    }

    public void UnequipBodyArmor()
    {
        if (instantiatedBodyArmor != null)
        {
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedBodyArmor.itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            PlayerController.HealthController.UpdateArmorAmount(-spawnedArmorData.armorAmount);
            OnArmorUnequipped?.Invoke(instantiatedBodyArmor.itemData.itemID);
            Destroy(instantiatedBodyArmor.instantiatedObject);
            instantiatedBodyArmor = null;
            PlayerController.PlayerContext.PlayerPreviewManager.UnequipArmorFromPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Body);

        }
    }

    public void EquipHelmet(ItemData itemData)
    {
        if (instantiatedHelmet == null)
        {
            GameObject obj = Instantiate(itemData.objectPrefab, helmetTransform.position, Quaternion.identity, helmetTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
            instantiatedHelmet = new EquippedArmor(obj, itemData);
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            PlayerController.HealthController.UpdateArmorAmount(spawnedArmorData.armorAmount);
            PlayerController.PlayerContext.PlayerPreviewManager.EquipArmorToPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Head, itemData.objectPrefab);
        }
        else
        {
            UnequipHelmet();
            EquipHelmet(itemData);
        }
    }

    public void UnequipHelmet()
    {
        if (instantiatedHelmet != null)
        {
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedHelmet.itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            PlayerController.HealthController.UpdateArmorAmount(-spawnedArmorData.armorAmount);
            OnArmorUnequipped?.Invoke(instantiatedHelmet.itemData.itemID);
            Destroy(instantiatedHelmet.instantiatedObject);
            instantiatedHelmet = null;
            PlayerController.PlayerContext.PlayerPreviewManager.UnequipArmorFromPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Head);
        }
    }

    public void EquipLegs(ItemData itemData)
    {
        if (instantiatedLegs == null)
        {
            GameObject obj = Instantiate(itemData.objectPrefab, legsTransform.position, Quaternion.identity, legsTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
            instantiatedLegs = new EquippedArmor(obj, itemData);
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            PlayerController.HealthController.UpdateArmorAmount(spawnedArmorData.armorAmount);
            PlayerController.PlayerContext.PlayerPreviewManager.EquipArmorToPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Legs, itemData.objectPrefab);
        }
        else
        {
            UnequipLegs();
            EquipHelmet(itemData);
        }
    }

    public void UnequipLegs()
    {
        if (instantiatedLegs != null)
        {
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedLegs.itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            PlayerController.HealthController.UpdateArmorAmount(-spawnedArmorData.armorAmount);
            OnArmorUnequipped?.Invoke(instantiatedLegs.itemData.itemID);
            Destroy(instantiatedLegs.instantiatedObject);
            instantiatedLegs = null;
            PlayerController.PlayerContext.PlayerPreviewManager.UnequipArmorFromPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Legs);
        }
    }


    public void EquipStarterItems(Item helmet, Item body, Item legs)
    {
        if (StarterItemsEquipped) return;
        StarterItemsEquipped = true;

        if (body != null)
        {
            Item starterBodyArmor = Instantiate(body);
            starterBodyArmor.itemData.itemID = PlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterBodyArmor.itemData);
            PlayerController.PlayerContext.InventoryController.AddItemToInventory(starterBodyArmor.itemData, true);
            Destroy(starterBodyArmor.gameObject);
        }

        if (helmet != null)
        {
            Item starterHelmet = Instantiate(helmet);
            starterHelmet.itemData.itemID = PlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterHelmet.itemData);
            PlayerController.PlayerContext.InventoryController.AddItemToInventory(starterHelmet.itemData, true);
            Destroy(starterHelmet.gameObject);
        }

        if (legs != null)
        {
            Item starterLegs = Instantiate(legs);
            starterLegs.itemData.itemID = PlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterLegs.itemData);
            PlayerController.PlayerContext.InventoryController.AddItemToInventory(starterLegs.itemData, true);
            Destroy(starterLegs.gameObject);
        }
    }

    public bool CanItemBeEquipped(ItemData itemData)
    {
        if (itemData.data is ArmorSO armorSO)
        {
            switch (armorSO.ArmorType)
            {
                case ArmorType.Light:
                    if (PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Heavy || PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Medium || PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Light)
                    {
                        return true;
                    }
                    return false;
                case ArmorType.Medium:
                    if (PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Heavy || PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Medium)
                    {
                        return true;
                    }
                    return false;
                case ArmorType.Heavy:
                    if (PlayerController.PlayerStatsBlackboard.armorType == ArmorType.Heavy)
                    {
                        return true;
                    }
                    return false;
            }
        }
        return false;
    }
}


