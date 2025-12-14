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
        switch (itemData.ItemSO.ItemType)
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
        switch (itemData.ItemSO.ItemType)
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
            GameObject obj = Instantiate(itemData.ItemPrefab, bodyArmorTransform.position, Quaternion.identity, bodyArmorTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
            instantiatedBodyArmor = new EquippedArmor(obj, itemData);
            if (itemData.ItemSO is ArmorSO)
                PlayerController.HealthController.UpdateArmorAmount(itemData.ArmorAmount);
            PlayerController.PlayerContext.PlayerPreviewManager.EquipArmorToPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Body, itemData.ItemPrefab);

            foreach (GemSocket gemSocket in itemData.socketedGems)
            {
                if (gemSocket.Gem != null)
                {
                    GemContext gemContext = new GemContext();
                    gemContext.playerContext = PlayerController.PlayerContext;
                    gemContext.itemData = itemData;
                    gemContext.slotType = gemSocket.SlotType;

                    foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                    {
                        if (gemEffect.AppliesTo == gemSocket.SlotType)
                        {
                            gemEffect.Apply(gemContext);
                        }
                    }
                }
            }
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
            if (instantiatedBodyArmor.itemData.ItemSO is ArmorSO)
                PlayerController.HealthController.UpdateArmorAmount(-instantiatedBodyArmor.itemData.ArmorAmount);
            OnArmorUnequipped?.Invoke(instantiatedBodyArmor.itemData.ItemID);
            foreach (GemSocket gemSocket in instantiatedBodyArmor.itemData.socketedGems)
            {
                foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                {
                    if (gemEffect.AppliesTo == gemSocket.SlotType)
                        gemEffect.Deregister();
                }
            }
            Destroy(instantiatedBodyArmor.instantiatedObject);
            instantiatedBodyArmor = null;
            PlayerController.PlayerContext.PlayerPreviewManager.UnequipArmorFromPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Body);
        }
    }

    public void EquipHelmet(ItemData itemData)
    {
        if (instantiatedHelmet == null)
        {
            GameObject obj = Instantiate(itemData.ItemPrefab, helmetTransform.position, Quaternion.identity, helmetTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
            instantiatedHelmet = new EquippedArmor(obj, itemData);
            if (itemData.ItemSO is ArmorSO)
                PlayerController.HealthController.UpdateArmorAmount(itemData.ArmorAmount);
            PlayerController.PlayerContext.PlayerPreviewManager.EquipArmorToPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Head, itemData.ItemPrefab);
            foreach (GemSocket gemSocket in itemData.socketedGems)
            {
                if (gemSocket.Gem != null)
                {
                    GemContext gemContext = new GemContext();
                    gemContext.playerContext = PlayerController.PlayerContext;
                    gemContext.itemData = itemData;
                    gemContext.slotType = gemSocket.SlotType;

                    foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                    {
                        if (gemEffect.AppliesTo == gemSocket.SlotType)
                        {
                            gemEffect.Apply(gemContext);
                        }
                    }
                }
            }
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
            if (instantiatedHelmet.itemData.ItemSO is ArmorSO)
                PlayerController.HealthController.UpdateArmorAmount(-instantiatedHelmet.itemData.ArmorAmount);
            OnArmorUnequipped?.Invoke(instantiatedHelmet.itemData.ItemID);
            foreach (GemSocket gemSocket in instantiatedHelmet.itemData.socketedGems)
            {
                foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                {
                    if (gemEffect.AppliesTo == gemSocket.SlotType)
                        gemEffect.Deregister();
                }
            }
            Destroy(instantiatedHelmet.instantiatedObject);
            instantiatedHelmet = null;
            PlayerController.PlayerContext.PlayerPreviewManager.UnequipArmorFromPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Head);
        }
    }

    public void EquipLegs(ItemData itemData)
    {
        if (instantiatedLegs == null)
        {
            GameObject obj = Instantiate(itemData.ItemPrefab, legsTransform.position, Quaternion.identity, legsTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
            instantiatedLegs = new EquippedArmor(obj, itemData);
            if (itemData.ItemSO is ArmorSO)
                PlayerController.HealthController.UpdateArmorAmount(itemData.ArmorAmount);
            PlayerController.PlayerContext.PlayerPreviewManager.EquipArmorToPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Legs, itemData.ItemPrefab);

            foreach (GemSocket gemSocket in itemData.socketedGems)
            {
                if (gemSocket.Gem != null)
                {
                    GemContext gemContext = new GemContext();
                    gemContext.playerContext = PlayerController.PlayerContext;
                    gemContext.itemData = itemData;
                    gemContext.slotType = gemSocket.SlotType;

                    foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                    {
                        if (gemEffect.AppliesTo == gemSocket.SlotType)
                        {
                            gemEffect.Apply(gemContext);
                        }
                    }
                }
            }
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
            if (instantiatedLegs.itemData.ItemSO is ArmorSO)
                PlayerController.HealthController.UpdateArmorAmount(-instantiatedLegs.itemData.ArmorAmount);
            OnArmorUnequipped?.Invoke(instantiatedLegs.itemData.ItemID);
            foreach (GemSocket gemSocket in instantiatedLegs.itemData.socketedGems)
            {
                foreach (GemEffectSO gemEffect in gemSocket.Gem.effects)
                {
                    if (gemEffect.AppliesTo == gemSocket.SlotType)
                        gemEffect.Deregister();
                }
            }
            Destroy(instantiatedLegs.instantiatedObject);
            instantiatedLegs = null;
            PlayerController.PlayerContext.PlayerPreviewManager.UnequipArmorFromPlayer(PlayerController.PlayerContext.PlayerIndex, ItemType.Legs);
        }
    }


    public void EquipStarterItems(ItemSO helmet, ItemSO body, ItemSO legs)
    {
        if (StarterItemsEquipped) return;
        StarterItemsEquipped = true;

        if (body != null)
        {
            ItemData itemData = SpawnedItemDataBase.Instance.CreateItemData(body, ItemQuality.Shoddy);
            PlayerController.PlayerContext.InventoryController.AddItemToInventory(itemData, true);
        }

        if (helmet != null)
        {
            ItemData itemData = SpawnedItemDataBase.Instance.CreateItemData(helmet, ItemQuality.Shoddy);
            PlayerController.PlayerContext.InventoryController.AddItemToInventory(itemData, true);
        }

        if (legs != null)
        {
            ItemData itemData = SpawnedItemDataBase.Instance.CreateItemData(legs, ItemQuality.Shoddy);
            PlayerController.PlayerContext.InventoryController.AddItemToInventory(itemData, true);
        }
    }

    public bool CanItemBeEquipped(ItemSO itemSO)
    {
        if (itemSO is ArmorSO armorSO)
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


