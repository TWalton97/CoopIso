using UnityEngine;
using TMPro;

public class StatsPanel : MonoBehaviour
{
    public PlayerStatsBlackboard playerStatsBlackboard;

    public TextMeshProUGUI MaximumHealthText;
    public TextMeshProUGUI CurrentHealthText;

    public TextMeshProUGUI MovementSpeedText;
    public TextMeshProUGUI CurrentSpeedText;

    public TextMeshProUGUI AttackCooldownText;

    public TextMeshProUGUI ResourceTypeText;
    public TextMeshProUGUI ResourceMaxText;
    public TextMeshProUGUI ResourceCurrentText;
    public TextMeshProUGUI ResourceMinText;

    private void Update()
    {
        if (playerStatsBlackboard == null) return;

        MaximumHealthText.text = "Maximum Health: " + playerStatsBlackboard.MaximumHealth.ToString();
        CurrentHealthText.text = "Current Health: " + playerStatsBlackboard.CurrentHealth.ToString();

        MovementSpeedText.text = "Movement Speed: " + playerStatsBlackboard.MovementSpeed.ToString();
        CurrentSpeedText.text = "Current Speed: " + playerStatsBlackboard.CurrentSpeed.ToString();

        AttackCooldownText.text = "Attack Cooldown: " + playerStatsBlackboard.AttackCooldown.ToString();

        ResourceTypeText.text = "Resource Type: " + playerStatsBlackboard.ResourceType.ToString();
        ResourceMaxText.text = "Maximum " + playerStatsBlackboard.ResourceType.ToString() + ": " + playerStatsBlackboard.ResourceMax.ToString();
        ResourceCurrentText.text = "Current " + playerStatsBlackboard.ResourceType.ToString() + ": " + playerStatsBlackboard.ResourceCurrent.ToString();
        ResourceMinText.text = "Minimum " + playerStatsBlackboard.ResourceType.ToString() + ": " + playerStatsBlackboard.ResourceMin.ToString();
    }
}
