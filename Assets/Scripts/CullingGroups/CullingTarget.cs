using UnityEngine;
using UnityEngine.Events;
using Utilities;
using UnityEngine.AI;
using System.Collections.Generic;

public enum CullingBehaviour { None, ToggleScripts, FadeInOut, Both }

public class CullingTarget : MonoBehaviour
{
    public UnityEvent onCulled, onVisible;
    public float boundarySphereRadius = 1f;
    public Renderer[] objectRenderers;
    public CullingBehaviour cullingMode = CullingBehaviour.FadeInOut;
    public bool isPriorityObject;

    MonoBehaviour[] scripts;

    NavMeshAgent agent;
    Animator animator;

    void Awake()
    {
        objectRenderers = gameObject.GetComponentsInChildren<Renderer>();
        scripts = GetComponents<MonoBehaviour>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        for (int i = 0; i < scripts.Length; i++)
        {
            if (scripts[i] == this) scripts[i] = null;
        }

    }

    void OnEnable()
    {
        if (isPriorityObject) onVisible?.Invoke();
        else TryRegister();
    }

    private void TryRegister()
    {
        if (CullingManager.Instance != null)
        {
            CullingManager.Instance.Register(this);
        }
        else
        {
            Invoke(nameof(TryRegister), 0.2f);
        }
    }

    void OnDisable()
    {
        if (!isPriorityObject) CullingManager.Instance.Deregister(this);
    }

    void EnableScripts(bool v)
    {
        for (int i = 0; i < scripts.Length; i++)
        {
            var s = scripts[i];
            if (s == null) continue;
            s.enabled = v;
        }
    }

    public void ToggleOn()
    {
        if (isPriorityObject)
        {
            onVisible?.Invoke();
            return;
        }

        switch (cullingMode)
        {
            case CullingBehaviour.ToggleScripts:
                EnableScripts(true);
                agent.enabled = true;
                animator.enabled = true;
                break;
            case CullingBehaviour.Both:
                EnableScripts(true);
                agent.enabled = true;
                animator.enabled = true;
                foreach (Renderer rend in objectRenderers)
                {
                    rend.enabled = true;
                }
                break;

        }
    }

    public void ToggleOff()
    {
        if (isPriorityObject) return;

        switch (cullingMode)
        {
            case CullingBehaviour.ToggleScripts:
                EnableScripts(false);
                agent.enabled = false;
                animator.enabled = false;
                break;
            case CullingBehaviour.Both:
                EnableScripts(false);
                agent.enabled = false;
                animator.enabled = false;
                foreach (Renderer rend in objectRenderers)
                {
                    rend.enabled = false;
                }
                break;
        }

        onCulled?.Invoke();
    }
}
