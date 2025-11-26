using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAimLineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Vector3 pointOnePosition;
    public LayerMask ObstructionMask;

    private bool isEnabled = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!isEnabled) return;

        UpdateLineRendererPosition();
    }

    public void ToggleLineRenderer(bool toggle)
    {
        isEnabled = toggle;
        lineRenderer.enabled = toggle;
    }

    private void UpdateLineRendererPosition()
    {
        lineRenderer.SetPosition(0, transform.position + transform.forward + pointOnePosition);
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward + pointOnePosition, transform.forward, out hit, 50f, ObstructionMask))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward + pointOnePosition + (transform.forward * 50f));
        }
    }
}
