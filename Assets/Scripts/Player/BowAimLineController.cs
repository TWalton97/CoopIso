using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAimLineController : MonoBehaviour
{
    public NewPlayerController PlayerController;
    private LineRenderer lineRenderer;
    public Vector3 pointOnePosition;
    public LayerMask ObstructionMask;

    private bool isEnabled = false;

    public GameObject HitTarget;


    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        PlayerController = GetComponentInParent<NewPlayerController>();
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
        Vector3 sphereCastDir;
        if (PlayerController.PlayerContext.PlayerInput.currentControlScheme == PlayerController.KEYBOARD_SCHEME)
        {
            sphereCastDir = PlayerController.lookPoint;
        }
        else
        {
            sphereCastDir = PlayerController.rotatedInputDirection;
        }

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.75f, sphereCastDir, out hit, 25f, ObstructionMask) && hit.collider.GetComponent<Entity>() != null)
        {
            HitTarget = hit.collider.gameObject;
        }
        else
        {
            HitTarget = null;
        }

        lineRenderer.SetPosition(0, transform.position + transform.forward + pointOnePosition);
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
