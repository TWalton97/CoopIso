using UnityEngine;
using Cinemachine;
using System.Collections;
using static UnityEngine.InputSystem.InputAction;
public class FreeLookCameraManager : Singleton<FreeLookCameraManager>
{
    public CinemachineFreeLook cinemachineFreeLook;
    public float mouseSensitivity = 0.15f;
    public float stickSensitivity = 120f;

    private bool mouseHeld;
    private Vector2 mouseDelta;
    public Vector2 stickDelta;

    public float rotationSpeed = 5f;

    private bool doSnapCamera = false;
    private float targetAngle;

    protected override void Awake()
    {
        base.Awake();
    }

    public void OnMouseDelta(CallbackContext ctx)
    {
        mouseDelta = ctx.ReadValue<Vector2>();
    }

    public void OnRotateButton(CallbackContext ctx)
    {
        Debug.Log($"{ctx.phase}");
        if (ctx.started)
        {
            mouseHeld = true;
        }
        else if (ctx.canceled)
        {
            mouseHeld = false;
        }
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        // Apply Gamepad rotation
        if (stickDelta.sqrMagnitude > 0.0001f)
        {
            cinemachineFreeLook.m_XAxis.Value += stickDelta.x * stickSensitivity * deltaTime;
        }

        // Apply Mouse Drag rotation
        if (mouseHeld && mouseDelta.sqrMagnitude > 0.00001f)
        {
            cinemachineFreeLook.m_XAxis.Value += mouseDelta.x * mouseSensitivity;
        }

        if (!doSnapCamera) return;

        cinemachineFreeLook.m_XAxis.Value = Quaternion.Lerp(Quaternion.Euler(0, cinemachineFreeLook.m_XAxis.Value, 0), Quaternion.Euler(0, targetAngle, 0), rotationSpeed * Time.deltaTime).eulerAngles.y;
        if (Mathf.Abs(cinemachineFreeLook.m_XAxis.Value - targetAngle) < 1f)
        {
            doSnapCamera = false;
        }
    }

    public void OrientTowardsLookDirection(Transform _target)
    {
        //cinemachineFreeLook.m_XAxis.Value = _target.transform.eulerAngles.y;
        targetAngle = _target.transform.rotation.eulerAngles.y;
        doSnapCamera = true;
    }
}
