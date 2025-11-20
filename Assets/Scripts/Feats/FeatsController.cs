using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatsController : MonoBehaviour
{
    //This controls what feats are available and which ones have been activated
    //In the feats UI menu, each of these feats will be represented by a button
    //Feats can have multiple levels available, as well as a required number of skillpoints 
    public NewPlayerController newPlayerController { get; private set; }
    public ExperienceController experienceController { get; private set; }
    public PlayerHealthController playerHealthController { get; private set; }

    public List<Feat> AvailableFeats;

    void Awake()
    {
        newPlayerController = GetComponent<NewPlayerController>();
    }

    void Start()
    {
        experienceController = newPlayerController.ExperienceController;
        playerHealthController = newPlayerController.PlayerHealthController;
    }

    public void ActivateFeat(int featIndex)
    {
        
    }

    [ContextMenu("Upgrade Vigor")]
    public void ActivateVigor()
    {
        ActivateFeat(0);
    }
}
