using System;
using UnityEngine;

public class ExperienceController : MonoBehaviour
{
    public int level = 1;
    public int experience = 0;

    public int[] levelExperienceRequirements;

    public Action OnExperienceGained;
    public Action OnLevelUp;

    public ParticleSystem LevelUpParticle;

    public ExperienceController(int _level = 1, int _experience = 0)
    {
        level = _level;
        experience = _experience;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        OnExperienceGained?.Invoke();
        if (experience > levelExperienceRequirements[level])
        {
            experience -= levelExperienceRequirements[level];
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level += 1;
        LevelUpParticle.Play();
        OnLevelUp?.Invoke();
    }
}
