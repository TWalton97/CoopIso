using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWorldSpaceUI : MonoBehaviour
{
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void LateUpdate()
    // Start is called before the first frame updateprivate void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }
}
