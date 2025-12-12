using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationAbilityBehaviour : SpellAbilityBehaviour
{
    public override void OnUse()
    {

    }
    public override void OnEnter()
    {
        player.PlayerStatsBlackboard.HasPreparationBuff = true;
        player.PlayerStatsBlackboard.PreparationCriticalDamageIncrease = runtime.Damage;
    }

    public override void OnExit()
    {

    }

    public override void OnChannelTick(float deltaTime)
    {

    }
}
