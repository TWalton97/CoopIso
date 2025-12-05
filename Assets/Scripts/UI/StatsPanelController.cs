using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsPanelController : MonoBehaviour
{
    public PlayerUserInterfaceController PlayerUserInterfaceController;
    private PlayerStatsBlackboard playerStatsBlackboard;
    private ExperienceController experienceController;
    private ResourceController resourceController;

    public TMP_Text CharacterName;
    public TMP_Text CharacterLevelAndClass;

    public TMP_Text CurrentExperience;
    public TMP_Text NextLevelExperience;
    public TMP_Text Health;
    public TMP_Text Mana;
    public TMP_Text Armor;
    public TMP_Text MovementSpeed;
    public TMP_Text CriticalChance;
    public TMP_Text CriticalDamage;
    public TMP_Text AttackSpeed;

    void Awake()
    {
        playerStatsBlackboard = PlayerUserInterfaceController.playerContext.PlayerController.PlayerStatsBlackboard;
        experienceController = PlayerUserInterfaceController.playerContext.PlayerController.ExperienceController;
        resourceController = PlayerUserInterfaceController.playerContext.PlayerController.ResourceController;
    }

    void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        CharacterLevelAndClass.text = "Level " + experienceController.level + " " + playerStatsBlackboard.ClassName;
        CurrentExperience.text = experienceController.experience.ToString();
        NextLevelExperience.text = experienceController.levelExperienceRequirements[experienceController.level + 1].ToString();
        Health.text = playerStatsBlackboard.CurrentHealth + "/" + playerStatsBlackboard.MaximumHealth + " [" + playerStatsBlackboard.HealthRegen + "/s]";
        Mana.text = resourceController.resource.resourceCurrent + "/" + resourceController.resource.resourceMax + " [" + resourceController.resource.RegenPerSecond + "/s]";
        Armor.text = playerStatsBlackboard.ArmorAmount.ToString();
        MovementSpeed.text = playerStatsBlackboard.MovementSpeed.ToString();
        CriticalChance.text = playerStatsBlackboard.CriticalChance.ToString() + "%";
        CriticalDamage.text = playerStatsBlackboard.CriticalDamage.ToString() + "%";
        AttackSpeed.text = playerStatsBlackboard.AttackSpeedMultiplier.ToString() + "%";
    }
}
