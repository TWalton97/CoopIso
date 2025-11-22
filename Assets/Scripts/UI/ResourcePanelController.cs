using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanelController : MonoBehaviour
{
    public PlayerHealthController PlayerHealthController;
    public ResourceController ResourceController;
    public ExperienceController ExperienceController;

    public Slider HealthBarFill;
    public Slider ResourceBarFill;
    public Slider ExperienceBarFill;

    public void Init(NewPlayerController playerController)
    {
        PlayerHealthController = playerController.PlayerHealthController;
        ResourceController = playerController.ResourceController;
        ExperienceController = playerController.ExperienceController;

        PlayerHealthController.OnTakeDamage += UpdateHealthBarFill;
        PlayerHealthController.OnHeal += UpdateHealthBarFill;
        ResourceController.resource.OnResourceValueChanged += UpdateResourceBarFill;
        ExperienceController.OnExperienceGained += UpdateExperienceBarFill;
        ExperienceController.OnLevelUp += UpdateExperienceBarFill;

        UpdateHealthBarFill(0);
        UpdateResourceBarFill();
        UpdateExperienceBarFill();
    }

    void OnDestroy()
    {
        PlayerHealthController.OnTakeDamage -= UpdateHealthBarFill;
        PlayerHealthController.OnHeal -= UpdateHealthBarFill;
        ResourceController.resource.OnResourceValueChanged -= UpdateResourceBarFill;
        ExperienceController.OnExperienceGained -= UpdateExperienceBarFill;
        ExperienceController.OnLevelUp -= UpdateExperienceBarFill;
    }

    private void UpdateHealthBarFill(int damage, Entity entity)
    {
        HealthBarFill.value = (float)PlayerHealthController.CurrentHealth / PlayerHealthController.MaximumHealth;
    }

    private void UpdateHealthBarFill(int amount)
    {
        HealthBarFill.value = (float)PlayerHealthController.CurrentHealth / PlayerHealthController.MaximumHealth;
    }

    private void UpdateResourceBarFill()
    {
        ResourceBarFill.value = (ResourceController.resource.resourceCurrent - ResourceController.resource.resourceMin) / (ResourceController.resource.resourceMax - ResourceController.resource.resourceMin);
    }

    private void UpdateExperienceBarFill()
    {
        ExperienceBarFill.value = (float)ExperienceController.experience / ExperienceController.levelExperienceRequirements[ExperienceController.level];
    }
}
