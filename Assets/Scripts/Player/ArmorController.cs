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
    public Item StartingBodyArmor;

    [Header("Helmet")]
    public Transform helmetTransform;
    private EquippedArmor instantiatedHelmet;
    public Item StartingHelmet;

    [Header("Legs")]
    public Transform legsTransform;
    private EquippedArmor instantiatedLegs;
    public Item StartingLegs;

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

    private void Start()
    {
        EquipStarterItems();
    }

    public void EquipBodyArmor(GameObject bodyArmorPrefab, ItemData itemData)
    {
        if (instantiatedBodyArmor == null)
        {
            GameObject obj = Instantiate(bodyArmorPrefab, bodyArmorTransform.position, Quaternion.identity, bodyArmorTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.RootBone;
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.Bones;
            instantiatedBodyArmor = new EquippedArmor(obj, itemData);
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            playerHealthController.UpdateArmorAmount(spawnedArmorData.armorAmount);
            PlayerPreviewManager.Instance.EquipArmorToPlayer(newPlayerController.PlayerInputController.playerIndex, ItemType.Body, bodyArmorPrefab);
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
            PlayerPreviewManager.Instance.UnequipArmorFromPlayer(newPlayerController.PlayerInputController.playerIndex, ItemType.Body);
        }
    }

    public void EquipHelmet(GameObject helmetPrefab, ItemData itemData)
    {
        if (instantiatedHelmet == null)
        {
            GameObject obj = Instantiate(helmetPrefab, helmetTransform.position, Quaternion.identity, helmetTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.RootBone;
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.Bones;
            instantiatedHelmet = new EquippedArmor(obj, itemData);
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            playerHealthController.UpdateArmorAmount(spawnedArmorData.armorAmount);
            PlayerPreviewManager.Instance.EquipArmorToPlayer(newPlayerController.PlayerInputController.playerIndex, ItemType.Head, helmetPrefab);
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
            PlayerPreviewManager.Instance.UnequipArmorFromPlayer(newPlayerController.PlayerInputController.playerIndex, ItemType.Head);
        }
    }

    public void EquipLegs(GameObject legsPrefab, ItemData itemData)
    {
        if (instantiatedLegs == null)
        {
            GameObject obj = Instantiate(legsPrefab, legsTransform.position, Quaternion.identity, legsTransform);
            SkinnedMeshRenderer skinnedMeshRenderer = obj.GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.rootBone = skinnedMeshRendererBoneRef.RootBone;
            skinnedMeshRenderer.bones = skinnedMeshRendererBoneRef.Bones;
            instantiatedLegs = new EquippedArmor(obj, itemData);
            SpawnedItemDataBase.SpawnedArmorData spawnedArmorData = newPlayerController.PlayerContext.SpawnedItemDatabase.GetSpawnedItemDataFromDataBase(itemData.itemID) as SpawnedItemDataBase.SpawnedArmorData;
            playerHealthController.UpdateArmorAmount(spawnedArmorData.armorAmount);
            PlayerPreviewManager.Instance.EquipArmorToPlayer(newPlayerController.PlayerInputController.playerIndex, ItemType.Legs, legsPrefab);
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
            PlayerPreviewManager.Instance.UnequipArmorFromPlayer(newPlayerController.PlayerInputController.playerIndex, ItemType.Legs);
        }
    }


    public void EquipStarterItems()
    {
        if (StarterItemsEquipped) return;
        StarterItemsEquipped = true;

        if (StartingBodyArmor != null)
        {
            Item starterBodyArmor = Instantiate(StartingBodyArmor);
            starterBodyArmor.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterBodyArmor.itemData);
            newPlayerController.PlayerContext.UserInterfaceController.inventoryController.FindEquippedSlotOfType(Slot.Body)[0].EquipGear(starterBodyArmor.itemData);
            Destroy(starterBodyArmor.gameObject);
        }

        if (StartingHelmet != null)
        {
            Item starterHelmet = Instantiate(StartingHelmet);
            starterHelmet.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterHelmet.itemData);
            newPlayerController.PlayerContext.UserInterfaceController.inventoryController.FindEquippedSlotOfType(Slot.Head)[0].EquipGear(starterHelmet.itemData);
            Destroy(starterHelmet.gameObject);
        }

        if (StartingLegs != null)
        {
            Item starterLegs = Instantiate(StartingLegs);
            starterLegs.itemData.itemID = newPlayerController.PlayerContext.SpawnedItemDatabase.RegisterItemToDatabase(starterLegs.itemData);
            newPlayerController.PlayerContext.UserInterfaceController.inventoryController.FindEquippedSlotOfType(Slot.Legs)[0].EquipGear(starterLegs.itemData);
            Destroy(starterLegs.gameObject);
        }
    }
}


