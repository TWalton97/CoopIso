using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GemEffectHandler
{
    public static ItemData SocketGem(GemSO Gem, ItemData itemData)
    {
        GemSocket gemSocket = new GemSocket();
        gemSocket.Gem = Gem;
        gemSocket.SlotType = itemData.EquipmentSlotType;

        itemData.socketedGems.Add(gemSocket);
        return itemData;
    }
}
