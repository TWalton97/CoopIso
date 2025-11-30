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
            SpawnedItemDataBase.SpawnedItemData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID);
            if (spawnedArmorData.itemData.data is ArmorSO armorData)
                PlayerController.HealthController.UpdateArmorAmount(LootCalculator.CalculateQualityModifiedStat(armorData.ArmorAmount, itemData.itemQuality));
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
            SpawnedItemDataBase.SpawnedItemData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedBodyArmor.itemData.itemID);
            if (spawnedArmorData.itemData.data is ArmorSO armorData)
                PlayerController.HealthController.UpdateArmorAmount(-LootCalculator.CalculateQualityModifiedStat(armorData.ArmorAmount, spawnedArmorData.itemQuality));
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
            SpawnedItemDataBase.SpawnedItemData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID);
            if (spawnedArmorData.itemData.data is ArmorSO armorData)
                PlayerController.HealthController.UpdateArmorAmount(LootCalculator.CalculateQualityModifiedStat(armorData.ArmorAmount, itemData.itemQuality));
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
            SpawnedItemDataBase.SpawnedItemData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedHelmet.itemData.itemID);
            if (spawnedArmorData.itemData.data is ArmorSO armorData)
                PlayerController.HealthController.UpdateArmorAmount(-LootCalculator.CalculateQualityModifiedStat(armorData.ArmorAmount, spawnedArmorData.itemQuality));
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
            SpawnedItemDataBase.SpawnedItemData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID);
            if (spawnedArmorData.itemData.data is ArmorSO armorData)
                PlayerController.HealthController.UpdateArmorAmount(LootCalculator.CalculateQualityModifiedStat(armorData.ArmorAmount, itemData.itemQuality));
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
            SpawnedItemDataBase.SpawnedItemData spawnedArmorData = PlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedLegs.itemData.itemID);
            if (spawnedArmorData.itemData.data is ArmorSO armorData)
                PlayerController.HealthController.UpdateArmorAmount(-LootCalculator.CalculateQualityModifiedStat(armorData.ArmorAmount, spawnedArmorData.itemQuality));
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
            starterBodyArmor.itemData.itemQuality = ItemQuality.Shoddy;
            starterBodyArmor.itemData.itemID = PlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterBodyArmor.itemData);
            PlayerController.PlayerContext.InventoryController.AddItemToInventory(starterBodyArmor.itemData, true);
            Destroy(starterBodyArmor.gameObject);
        }

        if (helmet != null)
        {
            Item starterHelmet = Instantiate(helmet);
            starterHelmet.itemData.itemQuality = ItemQuality.Shoddy;
            starterHelmet.itemData.itemID = PlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterHelmet.itemData);
            PlayerController.PlayerContext.InventoryController.AddItemToInventory(starterHelmet.itemData, true);
            Destroy(starterHelmet.gameObject);
        }

        if (legs != null)
        {
            Item starterLegs = Instantiate(legs);
            starterLegs.itemData.itemQuality = ItemQuality.Shoddy;
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


