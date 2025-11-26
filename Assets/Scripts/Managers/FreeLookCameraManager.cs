using UnityEngine;
using Cinemachine;
using System.Collections;
public class FreeLookCameraManager : Singleton<FreeLookCameraManager>
{
    public CinemachineFreeLook cinemachineFreeLook;
    public float rotationSpeed = 5f;

    private bool doSnapCamera = false;
    private float targetAngle;

    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (!doSnapCamera) return;

        cinemachineFreeLook.m_XAxis.Value = Quaternion.Lerp(Quaternion.Euler(0, cinemachineFreeLook.m_XAxis.Value, 0), Quaternion.Euler(0, targetAngle, 0), 5 * Time.deltaTime).eulerAngles.y;
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
