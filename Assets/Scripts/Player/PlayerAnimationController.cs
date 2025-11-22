using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public RuntimeAnimatorController baseController; // assign the shared base controller (asset)
    public List<AnimationClip> placeholderClips;     // assign the placeholder clips from the base controller

    private Animator animator;
    private AnimatorOverrideController runtimeAOC;
    // backing list used by Get/Apply overrides
    private List<KeyValuePair<AnimationClip, AnimationClip>> overridesList;
    // quick lookup: placeholder clip -> index in overridesList
    private Dictionary<AnimationClip, int> indexMap;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        // create per-player instance of the AOC
        runtimeAOC = new AnimatorOverrideController(baseController);
        animator.runtimeAnimatorController = runtimeAOC;

        // fetch initial overrides into a list
        overridesList = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        runtimeAOC.GetOverrides(overridesList);

        // build index map for fast lookup by placeholder clip reference
        indexMap = new Dictionary<AnimationClip, int>();
        for (int i = 0; i < overridesList.Count; i++)
        {
            var placeholder = overridesList[i].Key;
            if (placeholder != null && !indexMap.ContainsKey(placeholder))
                indexMap.Add(placeholder, i);
        }
    }

    /// <summary>
    /// Replace the animation for the given placeholder clip.
    /// Use placeholder clips you assigned from the base controller.
    /// </summary>
    public void SetOverrideByPlaceholderClip(AnimationClip placeholder, AnimationClip newClip)
    {
        if (placeholder == null)
        {
            Debug.LogWarning("Placeholder null in SetOverrideByPlaceholderClip");
            return;
        }

        if (!indexMap.TryGetValue(placeholder, out int idx))
        {
            // fallback: try to find by name (less robust)
            idx = overridesList.FindIndex(kv => kv.Key != null && kv.Key.name == placeholder.name);
            if (idx == -1)
            {
                Debug.LogWarning($"Placeholder clip '{placeholder.name}' not found in overrides.");
                return;
            }
            indexMap[placeholder] = idx;
        }

        // replace the value (value is the override clip)
        var original = overridesList[idx].Key;
        overridesList[idx] = new KeyValuePair<AnimationClip, AnimationClip>(original, newClip);

        runtimeAOC.ApplyOverrides(overridesList);
    }

    /// <summary>
    /// Convenience: replace based on placeholder name string (if you prefer).
    /// </summary>
    public void SetOverrideByPlaceholderName(string placeholderName, AnimationClip newClip)
    {
        int idx = overridesList.FindIndex(kv => kv.Key != null && kv.Key.name == placeholderName);
        if (idx == -1)
        {
            Debug.LogWarning($"Placeholder name '{placeholderName}' not found in overrides.");
            return;
        }

        var original = overridesList[idx].Key;
        overridesList[idx] = new KeyValuePair<AnimationClip, AnimationClip>(original, newClip);
        runtimeAOC.ApplyOverrides(overridesList);
    }

    /// <summary>
    /// Remove override (reset to original) for a placeholder clip.
    /// </summary>
    public void ClearOverride(AnimationClip placeholder)
    {
        if (!indexMap.TryGetValue(placeholder, out int idx))
        {
            idx = overridesList.FindIndex(kv => kv.Key != null && kv.Key.name == placeholder.name);
            if (idx == -1) return;
            indexMap[placeholder] = idx;
        }

        var original = overridesList[idx].Key;
        overridesList[idx] = new KeyValuePair<AnimationClip, AnimationClip>(original, original);
        runtimeAOC.ApplyOverrides(overridesList);
    }

    /// <summary>
    /// Get the currently assigned override clip for a placeholder (may return null).
    /// </summary>
    public AnimationClip GetCurrentOverride(AnimationClip placeholder)
    {
        if (!indexMap.TryGetValue(placeholder, out int idx))
        {
            idx = overridesList.FindIndex(kv => kv.Key != null && kv.Key.name == placeholder.name);
            if (idx == -1) return null;
            indexMap[placeholder] = idx;
        }

        return overridesList[idx].Value;
    }
}
