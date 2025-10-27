using UnityEngine;
using System;

public class AbilityController : MonoBehaviour
{
    public Action OnAbilityStarted;
    public Action OnAbilityFinished;

    public BaseAbility ability;

    public void UseAbility()
    {
        //Players presses the ability button
        //Call gets sent to the ability controller, which checks if the conditions are met to use the ability -> Is there an ability? Does the ability require mana?
        //If the conditions are met, the resources are taken from the player, the ability gets casted, and the OnAbilityStarted action is invoked
        //Once the ability is completed, the OnAbilityFinished gets invoked

    }
}
