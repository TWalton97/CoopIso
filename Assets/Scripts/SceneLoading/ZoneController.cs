using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public int ZoneLevel;
    public Enemy[] Enemies;
    public Chest[] Chests;

    public void SetupZone()
    {
        Debug.Log("Setting up zone");
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
}
