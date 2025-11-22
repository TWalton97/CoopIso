using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityScrollController : MonoBehaviour
{
    public List<BaseAbility> abilities;          // Full ability list
    public List<RectTransform> iconSlots;            // UI icon images (in left→right order)
    public float slideDuration = 0.15f;
    public float slotSpacing = 100f;  // spacing between icons (set in inspector)

    private int centerIndex = 2;
    private bool isAnimating = false;

    void Start()
    {
        RefreshIcons();
    }

    [ContextMenu("CycleRight")]
    public void CycleRight() // RB pressed
    {
        if (!isAnimating)
        {
            centerIndex = (centerIndex + 1) % iconSlots.Count;
            StartCoroutine(SlideIcons(-1));
        }

    }

    [ContextMenu("CycleLeft")]
    public void CycleLeft() // LB pressed
    {
        if (!isAnimating)
        {
            centerIndex = (centerIndex - 1 + iconSlots.Count) % iconSlots.Count;
            StartCoroutine(SlideIcons(+1));
        }

    }

    IEnumerator SlideIcons(int direction)
    {
        isAnimating = true;

        Vector2[] startPos = new Vector2[iconSlots.Count];
        Vector2[] targetPos = new Vector2[iconSlots.Count];

        // Set start and destination positions
        for (int i = 0; i < iconSlots.Count; i++)
        {
            startPos[i] = iconSlots[i].anchoredPosition;
            targetPos[i] = startPos[i] + new Vector2(direction * slotSpacing, 0);
        }

        float t = 0;

        // Animate using Lerp
        while (t < 1f)
        {
            t += Time.deltaTime / slideDuration;
            float lerp = Mathf.SmoothStep(0, 1, t);

            for (int i = 0; i < iconSlots.Count; i++)
            {
                // Move icons
                iconSlots[i].anchoredPosition = Vector2.Lerp(startPos[i], targetPos[i], lerp);

                // --- NEW: Animate scale ---
                float startSlotIndex = GetSlotOffsetFromCenter(startPos[i].x);
                float endSlotIndex = GetSlotOffsetFromCenter(targetPos[i].x);
                float slotBlend = Mathf.Lerp(startSlotIndex, endSlotIndex, lerp);

                float scale = 1.0f - (Mathf.Abs(slotBlend) * 0.15f);
                iconSlots[i].localScale = Vector3.one * scale;
            }

            yield return null;
        }

        // Update the selected index once animation finishes
        centerIndex = (centerIndex - direction + abilities.Count) % abilities.Count;

        // Reposition icons to clean slots
        PositionIconsInstantly();

        // Refresh sprites for new ability positions
        RefreshIcons();

        isAnimating = false;
    }

    // ============================
    // SUPPORT FUNCTIONS
    // ============================

    void PositionIconsInstantly()
    {
        int half = iconSlots.Count / 2;

        for (int i = 0; i < iconSlots.Count; i++)
        {
            float x = (i - half) * slotSpacing;
            iconSlots[i].anchoredPosition = new Vector2(x, 0);
        }
    }

    void RefreshIcons()
    {
        int half = iconSlots.Count / 2;

        for (int i = 0; i < iconSlots.Count; i++)
        {
            int offset = i - half;
            int abilityIndex = (centerIndex + offset + abilities.Count) % abilities.Count;

            AbilityCell cell = iconSlots[i].GetComponentInChildren<AbilityCell>();
            cell.icon.sprite = abilities[abilityIndex].abilityData.AbilityIcon;
        }
    }

    float GetSlotOffsetFromCenter(float x)
    {
        // Converts x position into a "slot offset" like -2, -1, 0, +1, +2
        return x / slotSpacing;
    }
}
