using UnityEngine;
using System.Collections.Generic;

public class ZoneController : MonoBehaviour
{
    public Dictionary<int, Checkpoint> registeredCheckpoints = new();

    public int ZoneLevel;
    public Enemy[] Enemies;
    public Chest[] Chests;

    public void SetupZone()
    {
        Enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in Enemies)
        {
            enemy.Level = ZoneLevel;
            enemy.ApplyStats();
        }

        Chests = FindObjectsOfType<Chest>();
        foreach (Chest chest in Chests)
        {
            chest.minBudget *= ZoneLevel;
            chest.maxBudget *= ZoneLevel;
        }
    }

    public void RegisterCheckpoint(Checkpoint checkpoint)
    {
        if (!registeredCheckpoints.ContainsKey(checkpoint.CheckpointIndex))
            registeredCheckpoints.Add(checkpoint.CheckpointIndex, checkpoint);
    }

    public Checkpoint FindCheckpointByIndex(int index)
    {
        return registeredCheckpoints[index];
    }
}
