using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorController : MonoBehaviour
{
    //Stores a reference to each transform
    //Handles instantiating prefabs in their correct position and storing data
    public NewPlayerController newPlayerController;
    public PlayerHealthController playerHealthController;
    public SkinnedMeshRendererBoneRef skinnedMeshRendererBoneRef;

    [Header("Body Armor")]
    public Transform bodyArmorTransform;
    private EquippedArmor instantiatedBodyArmor;

    [Header("Helmet")]
    public Transform helmetTransform;
    private EquippedArmor instantiatedHelmet;

    [Header("Legs")]
    public Transform legsTransform;
    private EquippedArmor instantiatedLegs;

    private bool StarterItemsEquipped = false;

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

    public void EquipBodyArmor(GameObject bodyArmorPrefab, ItemData itemData)
    {
        if (instantiatedBodyArmor == null)
        {
            GameObject obj = Instantiate(bodyArmorPrefab, bodyArmorTransform.position, Quaternion.identity, bodyArmorTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
            instantiatedBodyArmor = new EquippedArmor(obj, itemData);
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            playerHealthController.UpdateArmorAmount(spawnedArmorData.armorAmount);
            newPlayerController.PlayerContext.PlayerPreviewManager.EquipArmorToPlayer(newPlayerController.PlayerContext.PlayerIndex, ItemType.Body, bodyArmorPrefab);
        }
    }

    public void UnequipBodyArmor()
    {
        if (instantiatedBodyArmor != null)
        {
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedBodyArmor.itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            playerHealthController.UpdateArmorAmount(-spawnedArmorData.armorAmount);
            Destroy(instantiatedBodyArmor.instantiatedObject);
            instantiatedBodyArmor = null;
            newPlayerController.PlayerContext.PlayerPreviewManager.UnequipArmorFromPlayer(newPlayerController.PlayerContext.PlayerIndex, ItemType.Body);
        }
    }

    public void EquipHelmet(GameObject helmetPrefab, ItemData itemData)
    {
        if (instantiatedHelmet == null)
        {
            GameObject obj = Instantiate(helmetPrefab, helmetTransform.position, Quaternion.identity, helmetTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
            instantiatedHelmet = new EquippedArmor(obj, itemData);
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            playerHealthController.UpdateArmorAmount(spawnedArmorData.armorAmount);
            newPlayerController.PlayerContext.PlayerPreviewManager.EquipArmorToPlayer(newPlayerController.PlayerContext.PlayerIndex, ItemType.Head, helmetPrefab);
        }
    }

    public void UnequipHelmet()
    {
        if (instantiatedHelmet != null)
        {
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedHelmet.itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            playerHealthController.UpdateArmorAmount(-spawnedArmorData.armorAmount);
            Destroy(instantiatedHelmet.instantiatedObject);
            instantiatedHelmet = null;
            newPlayerController.PlayerContext.PlayerPreviewManager.UnequipArmorFromPlayer(newPlayerController.PlayerContext.PlayerIndex, ItemType.Head);
        }
    }

    public void EquipLegs(GameObject legsPrefab, ItemData itemData)
    {
        if (instantiatedLegs == null)
        {
            GameObject obj = Instantiate(legsPrefab, legsTransform.position, Quaternion.identity, legsTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.GetRootBone();
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.GetBones();
            instantiatedLegs = new EquippedArmor(obj, itemData);
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            playerHealthController.UpdateArmorAmount(spawnedArmorData.armorAmount);
            newPlayerController.PlayerContext.PlayerPreviewManager.EquipArmorToPlayer(newPlayerController.PlayerContext.PlayerIndex, ItemType.Legs, legsPrefab);
        }
    }

    public void UnequipLegs()
    {
        if (instantiatedLegs != null)
        {
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(instantiatedLegs.itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            playerHealthController.UpdateArmorAmount(-spawnedArmorData.armorAmount);
            Destroy(instantiatedLegs.instantiatedObject);
            instantiatedLegs = null;
            newPlayerController.PlayerContext.PlayerPreviewManager.UnequipArmorFromPlayer(newPlayerController.PlayerContext.PlayerIndex, ItemType.Legs);
        }
    }


    public void EquipStarterItems(Item helmet, Item body, Item legs)
    {
        if (StarterItemsEquipped) return;
        StarterItemsEquipped = true;

        if (body != null)
        {
            Item starterBodyArmor = Instantiate(body);
            starterBodyArmor.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterBodyArmor.itemData);
            newPlayerController.PlayerContext.UserInterfaceController.inventoryController.FindEquippedSlotOfType(Slot.Body)[0].EquipGear(starterBodyArmor.itemData, newPlayerController);
            Destroy(starterBodyArmor.gameObject);
        }

        if (helmet != null)
        {
            Item starterHelmet = Instantiate(helmet);
            starterHelmet.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterHelmet.itemData);
            newPlayerController.PlayerContext.UserInterfaceController.inventoryController.FindEquippedSlotOfType(Slot.Head)[0].EquipGear(starterHelmet.itemData, newPlayerController);
            Destroy(starterHelmet.gameObject);
        }

        if (legs != null)
        {
            Item starterLegs = Instantiate(legs);
            starterLegs.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterLegs.itemData);
            newPlayerController.PlayerContext.UserInterfaceController.inventoryController.FindEquippedSlotOfType(Slot.Legs)[0].EquipGear(starterLegs.itemData, newPlayerController);
            Destroy(starterLegs.gameObject);
        }
    }
}


