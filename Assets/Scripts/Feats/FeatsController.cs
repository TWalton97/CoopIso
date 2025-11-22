using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatsController : MonoBehaviour
{
    public NewPlayerController newPlayerController;
    public ExperienceController experienceController;
    public PlayerHealthController playerHealthController;

    public List<Feat> AvailableFeats = new List<Feat>();

    void Awake()
    {
        AddFeatsToList();
    }

    public void ActivateFeat(int featIndex, Action activatedSuccess, bool bypassReqs = false)
    {
        if (bypassReqs)
        {
            AvailableFeats[featIndex].OnActivateNoReqs(this, activatedSuccess);
        }
        else
        {
            AvailableFeats[featIndex].OnActivate(this, activatedSuccess);
        }
    }


    private void AddFeatsToList()
    {
        Vigor vigor = new Vigor(0);
        AvailableFeats.Add(vigor);
        AvailableFeats.Add(vigor);
        AvailableFeats.Add(vigor);
        Nimble nimble = new Nimble(0);
        AvailableFeats.Add(nimble);
        AvailableFeats.Add(nimble);
        AvailableFeats.Add(nimble);
        ShieldTraining shieldTraining = new ShieldTraining(0);
        AvailableFeats.Add(shieldTraining);
        DualWieldMastery dualWieldMastery = new DualWieldMastery(0);
        AvailableFeats.Add(dualWieldMastery);
        TwoHandedMastery twoHandedMastery = new TwoHandedMastery(0);
        AvailableFeats.Add(twoHandedMastery);
        ArmorMastery armorMastery = new ArmorMastery(1);
        AvailableFeats.Add(armorMastery);
    }

}
