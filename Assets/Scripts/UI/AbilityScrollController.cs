using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityScrollController : MonoBehaviour
{
    public PlayerUserInterfaceController controller;

    [Header("References")]
    public List<RectTransform> iconSlots;
    public List<AbilityCell> iconCells;

    [Header("Ability Data")]
    public AbilityData ActiveAbility;

    [Header("Ability Description")]
    public GameObject AbilityDescriptionParent;
    public TMP_Text AbilityName;
    public TMP_Text AbilityCost;
    public TMP_Text AbilityDescription;
    public float DisplayDuration;
    private float currentDuration;

    [Header("Settings")]
    public float slotSpacing = 125f;
    public float slideDuration = 1.25f;
    public float centerScale = 1.0f;
    public float sideScale = 0.75f;

    private int centerIndex = 0;
    private bool isAnimating = false;

    public List<AbilityData> Abilities = new();

    public class AbilityData
    {
        public AbilitySO AbilitySO;
        public AbilityBehaviourBase AbilityBehaviour;

        public AbilityData(AbilitySO _abilitySO, AbilityBehaviourBase _abilityBehaviour)
        {
            AbilitySO = _abilitySO;
            AbilityBehaviour = _abilityBehaviour;
        }
    }

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

    void Update()
    {
        if (currentDuration <= 0)
        {
            if (AbilityDescriptionParent.activeSelf)
            {
                ToggleAbilityDescription(false);
            }
            return;
        }

        currentDuration -= Time.deltaTime;
    }

    // Rebuild the whole thing when abilities change
    public void RebuildCarousel()
    {
        centerIndex = Mathf.Clamp(centerIndex, 0, Abilities.Count - 1);
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
        if (Abilities.Count == 0) return;

        int half = iconSlots.Count / 2;

        for (int slot = 0; slot < iconSlots.Count; slot++)
        {
            int offset = slot - half;

            // Wrap ability index
            int abilityIndex = (centerIndex + offset + Abilities.Count) % Abilities.Count;

            iconCells[slot].icon.sprite = Abilities[abilityIndex].AbilitySO.AbilityIcon;
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
        if (!isAnimating && Abilities.Count > 1)
            StartCoroutine(SlideIcons(+1));

        if (Abilities.Count > 0)
        {
            AbilityData nextAbility = Abilities[(centerIndex + 1 + Abilities.Count) % Abilities.Count];
            UpdateAbilityDescription(nextAbility);
        }
    }

    [ContextMenu("CycleLeft")]
    public void CycleLeft()
    {
        if (!isAnimating && Abilities.Count > 1)
            StartCoroutine(SlideIcons(-1));

        if (Abilities.Count > 0)
        {
            AbilityData nextAbility = Abilities[(centerIndex + -1 + Abilities.Count) % Abilities.Count];

            UpdateAbilityDescription(nextAbility);
        }
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
        centerIndex = (centerIndex + direction + Abilities.Count) % Abilities.Count;

        LayoutSlots();
        RefreshIcons();
        ApplyInstantScales();
        ActiveAbility = Abilities[centerIndex];

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

    public void AddAbility(AbilitySO ability, AbilityBehaviourBase abilityBehaviour)
    {
        AbilityData abilityData = new AbilityData(ability, abilityBehaviour);
        if (Abilities.Count == 0)
        {
            ActiveAbility = abilityData;
            UpdateAbilityDescription(abilityData);
        }
        Abilities.Add(abilityData);

        RebuildCarousel();
    }



    public bool AbilityReadyToBeUsed()
    {
        if (ActiveAbility == null) return false;

        if (!ActiveAbility.AbilityBehaviour.CanUse(controller.inventoryController.FeatsMenu.playerController.ResourceController)) return false;

        return true;
    }

    public void UpdateAbilityDescription(AbilityData ability)
    {
        AbilityName.text = ability.AbilitySO.AbilityName;
        AbilityCost.text = ability.AbilitySO.ResourceAmount.ToString() + " " + ability.AbilitySO.ResourceType.ToString();
        RuntimeAbility runtime = controller.playerContext.PlayerController.AbilityController.GetRuntime(ability.AbilitySO);

        if (ability.AbilityBehaviour is WeaponAbilityBehaviour weaponAbilityBehaviour)
        {
            int damage = weaponAbilityBehaviour.CalculateDamagePerTick();
            AbilityDescription.text = ability.AbilitySO.GetCalculatedLevelDescription(runtime.currentLevel, damage);
        }
        else
        {
            AbilityDescription.text = ability.AbilitySO.GetLevelDescription(runtime.currentLevel);
        }

        ToggleAbilityDescription(true);
    }

    public void ToggleAbilityDescription(bool toggle)
    {
        AbilityDescriptionParent.SetActive(toggle);
        currentDuration = DisplayDuration;
    }
}
