using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GemEffectHandler
{
    public static ItemData SocketGem(GemSO Gem, ItemData itemData)
    {
        if (itemData.currentSockets >= itemData.socketedGems.Count)
        {
            GemSocket gemSocket = new GemSocket();
            gemSocket.gem = Gem;

            var entry = Gem.GetEffectForSlot(itemData.EquipmentSlotType);
            Type type = Type.GetType(entry.EffectClassName);

            if (type == null)
                return null;

            IGemEffect effect = Activator.CreateInstance(type) as IGemEffect;

            gemSocket.gemEffect = effect;

            gemSocket.hitVFX = entry.HitVFX;
            gemSocket.weaponVFX = entry.WeaponVFX;
            itemData.socketedGems.Add(gemSocket);
        }
        return itemData;
    }
}
