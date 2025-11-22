using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScrollController : MonoBehaviour
{
    public PlayerUserInterfaceController controller;

    [Header("References")]
    public List<RectTransform> iconSlots;         // Always size 5
    public List<AbilityCell> iconCells;           // Always size 5

    [Header("Ability Data")]
    public List<BaseAbility> abilities = new List<BaseAbility>();
    public BaseAbility ActiveAbility;

    [Header("Settings")]
    public float slotSpacing = 125f;
    public float slideDuration = 1.25f;
    public float centerScale = 1.0f;
    public float sideScale = 0.75f;

    private int centerIndex = 0;   // Index in abilities list
    private bool isAnimating = false;

    // ================================
    // INIT
    // ================================
    void Start()
    {
        controller = GetComponentInParent<PlayerUserInterfaceController>();
        LayoutSlots();
        RefreshIcons();
        ApplyInstantScales();
    }

    // Rebuild the whole thing when abilities change
    public void RebuildCarousel()
    {
        centerIndex = Mathf.Clamp(centerIndex, 0, abilities.Count - 1);
        RefreshIcons();
        ApplyInstantScales();
    }

    // ================================
    // SLOT LAYOUT
    // ================================
    void LayoutSlots()
    {
        int half = iconSlots.Count / 2;

        for (int i = 0; i < iconSlots.Count; i++)
        {
            float x = (i - half) * slotSpacing;
            iconSlots[i].anchoredPosition = new Vector2(x, 0);
        }
    }

    // ================================
    // REFRESH ICON SPRITES
    // ================================
    void RefreshIcons()
    {
        if (abilities.Count == 0) return;

        int half = iconSlots.Count / 2;

        for (int slot = 0; slot < iconSlots.Count; slot++)
        {
            int offset = slot - half;

            // Wrap ability index
            int abilityIndex = (centerIndex + offset + abilities.Count) % abilities.Count;

            iconCells[slot].icon.sprite = abilities[abilityIndex].abilityData.AbilityIcon;
        }
    }

    // ================================
    // SCALE
    // ================================
    void ApplyInstantScales()
    {
        int half = iconSlots.Count / 2;

        for (int slot = 0; slot < iconSlots.Count; slot++)
        {
            int offset = slot - half;
            bool isCenter = (offset == 0);

            float targetScale = isCenter ? centerScale : sideScale;
            iconSlots[slot].localScale = Vector3.one * targetScale;
        }
    }

    // ================================
    // CYCLING
    // ================================
    [ContextMenu("CycleRight")]
    public void CycleRight()
    {
        if (!isAnimating && abilities.Count > 1)
            StartCoroutine(SlideIcons(+1));
    }

    [ContextMenu("CycleLeft")]
    public void CycleLeft()
    {
        if (!isAnimating && abilities.Count > 1)
            StartCoroutine(SlideIcons(-1));
    }

    // ================================
    // MAIN SLIDE ANIMATION
    // ================================
    IEnumerator SlideIcons(int direction)
    {
        isAnimating = true;

        Vector2[] startPos = new Vector2[iconSlots.Count];
        Vector2[] endPos = new Vector2[iconSlots.Count];

        for (int i = 0; i < iconSlots.Count; i++)
        {
            startPos[i] = iconSlots[i].anchoredPosition;

            // Move icons visually opposite to direction of selection
            endPos[i] = startPos[i] + new Vector2(-direction * slotSpacing, 0);
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / slideDuration;
            float lerp = Mathf.SmoothStep(0, 1, t);

            AnimateSlots(startPos, endPos, lerp, direction);

            yield return null;
        }

        // Update selected ability
        centerIndex = (centerIndex + direction + abilities.Count) % abilities.Count;

        LayoutSlots();
        RefreshIcons();
        ApplyInstantScales();
        ActiveAbility = abilities[centerIndex];

        isAnimating = false;
    }

    // Movement + scaling logic
    void AnimateSlots(Vector2[] start, Vector2[] end, float lerp, int direction)
    {
        int half = iconSlots.Count / 2;

        for (int slot = 0; slot < iconSlots.Count; slot++)
        {
            // Move
            iconSlots[slot].anchoredPosition = Vector2.Lerp(start[slot], end[slot], lerp);

            // Animated logical slot index
            float startSlot = slot - half;
            float endSlot = startSlot - direction;

            float blendedSlot = Mathf.Lerp(startSlot, endSlot, lerp);

            // Scale calculation
            float scale = Mathf.Lerp(centerScale, sideScale, Mathf.Abs(blendedSlot));
            iconSlots[slot].localScale = Vector3.one * scale;
        }
    }

    public void AddAbility(BaseAbility ability)
    {
        if (abilities.Count == 0)
            ActiveAbility = ability;
        abilities.Add(ability);
        RebuildCarousel();
    }

    public bool AbilityReadyToBeUsed()
    {
        if (ActiveAbility == null) return false;

        if (!ActiveAbility.CanUse(controller.featsPanelController.playerController.ResourceController)) return false;

        return true;
    }
}
