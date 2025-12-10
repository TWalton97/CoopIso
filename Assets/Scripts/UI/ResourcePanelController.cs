using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelController : MonoBehaviour
{
    public HealthController HealthController;
    public ResourceController ResourceController;
    public ExperienceController ExperienceController;

    public Image HealthBarFill;
    public Image ResourceBarFill;
    public Image ExperienceBarFill;
    public GameObject SkillPointNotification;

    public Image HealthBarHealingFill;
    public Image ResourceBarRestoreFill;

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
        HealthBarFill.fillAmount = (float)HealthController.CurrentHealth / HealthController.MaximumHealth;
        HealthBarHealingFill.fillAmount = (float)Mathf.Clamp01((HealthController.CurrentHealth + HealthController.remainingRestoreAmount) / HealthController.MaximumHealth);
    }

    private void UpdateHealthBarFill(int amount)
    {
        HealthBarFill.fillAmount = (float)HealthController.CurrentHealth / HealthController.MaximumHealth;
        HealthBarHealingFill.fillAmount = (float)Mathf.Clamp01((HealthController.CurrentHealth + HealthController.remainingRestoreAmount) / HealthController.MaximumHealth);
    }

    private void UpdateResourceBarFill()
    {
        ResourceBarFill.fillAmount = (float)(ResourceController.resource.resourceCurrent - ResourceController.resource.resourceMin) / (ResourceController.resource.resourceMax - ResourceController.resource.resourceMin);
        ResourceBarRestoreFill.fillAmount = (float)Mathf.Clamp01((ResourceController.resource.resourceCurrent + ResourceController.resource.remainingRestoreAmount - ResourceController.resource.resourceMin) / (ResourceController.resource.resourceMax - ResourceController.resource.resourceMin));
    }

    private void UpdateExperienceBarFill()
    {
        ExperienceBarFill.fillAmount = (float)ExperienceController.experience / ExperienceController.levelExperienceRequirements[ExperienceController.level];
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
