using UnityEngine;
using System.Collections.Generic;

public class AttackSlotManager : MonoBehaviour
{
    [SerializeField] private int maxSlots = 8;        // maximum slots around player
    [SerializeField] private float radius = 1.0f;    // distance from player

    private List<Transform> slots;

    private void Awake()
    {
        slots = new List<Transform>(maxSlots);
        for (int i = 0; i < maxSlots; i++)
            slots.Add(null); // null = free slot
    }

    // Pick the slot that has the largest angular separation from existing occupied slots
    public int RequestSlotDynamic(Vector3 enemyPosition)
    {
        int bestIndex = -1;
        float bestScore = -1f;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] != null) continue; // already occupied

            float angle = i * 360f / slots.Count;
            float minAngleDiff = float.MaxValue;
            bool tooClose = false;

            // Check distance to all occupied slots
            for (int j = 0; j < slots.Count; j++)
            {
                if (slots[j] == null) continue;
                float occupiedAngle = j * 360f / slots.Count;
                float diff = Mathf.Abs(Mathf.DeltaAngle(angle, occupiedAngle));
                if (diff < 20f) // minimum separation threshold
                {
                    tooClose = true;
                    break;
                }
                if (diff < minAngleDiff) minAngleDiff = diff;
            }

            if (tooClose) continue;

            if (minAngleDiff > bestScore)
            {
                bestScore = minAngleDiff;
                bestIndex = i;
            }
        }

        if (bestIndex != -1)
            slots[bestIndex] = null; // mark as temporarily occupied

        return bestIndex;
    }

    public Vector3 GetSlotPosition(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
            return transform.position;

        float angle = slotIndex * 360f / slots.Count;
        return transform.position + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
    }

    public void ReleaseSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < slots.Count)
            slots[slotIndex] = null;
    }
}
