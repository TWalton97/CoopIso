using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelController : MonoBehaviour
{
    public HealthController HealthController;
    public ResourceController ResourceController;
    public ExperienceController ExperienceController;

    public Slider HealthBarFill;
    public Slider ResourceBarFill;
    public Slider ExperienceBarFill;
    public GameObject SkillPointNotification;

    public void Init(NewPlayerController playerController)
    {
        HealthController = playerController.HealthController;
        ResourceController = playerController.ResourceController;
        ExperienceController = playerController.ExperienceController;

        HealthController.OnTakeDamage += UpdateHealthBarFill;
        HealthController.OnHeal += UpdateHealthBarFill;
        ResourceController.resource.OnResourceValueChanged += UpdateResourceBarFill;
        ExperienceController.OnExperienceGained += UpdateExperienceBarFill;
        ExperienceController.OnLevelUp += UpdateExperienceBarFill;
        ExperienceController.OnSkillPointUsed += CheckToEnableSkillPointNotification;

        UpdateHealthBarFill(0);
        UpdateResourceBarFill();
        UpdateExperienceBarFill();
    }

    void OnDestroy()
    {
        HealthController.OnTakeDamage -= UpdateHealthBarFill;
        HealthController.OnHeal -= UpdateHealthBarFill;
        ResourceController.resource.OnResourceValueChanged -= UpdateResourceBarFill;
        ExperienceController.OnExperienceGained -= UpdateExperienceBarFill;
        ExperienceController.OnLevelUp -= UpdateExperienceBarFill;
    }

    private void UpdateHealthBarFill(int damage, Entity entity)
    {
        HealthBarFill.value = (float)HealthController.CurrentHealth / HealthController.MaximumHealth;
    }

    private void UpdateHealthBarFill(int amount)
    {
        HealthBarFill.value = (float)HealthController.CurrentHealth / HealthController.MaximumHealth;
    }

    private void UpdateResourceBarFill()
    {
        ResourceBarFill.value = (ResourceController.resource.resourceCurrent - ResourceController.resource.resourceMin) / (ResourceController.resource.resourceMax - ResourceController.resource.resourceMin);
    }

    private void UpdateExperienceBarFill()
    {
        ExperienceBarFill.value = (float)ExperienceController.experience / ExperienceController.levelExperienceRequirements[ExperienceController.level];
        CheckToEnableSkillPointNotification();
    }

    private void CheckToEnableSkillPointNotification()
    {
        if (ExperienceController.SkillPoints == 0)
        {
            SkillPointNotification.SetActive(false);
        }
        else
        {
            SkillPointNotification.SetActive(true);
        }
    }
}
