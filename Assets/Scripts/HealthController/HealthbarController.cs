using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
    public HealthController HealthController;
    public Image HealthbarFill;

    void Awake()
    {
        if (HealthController == null) HealthController = GetComponentInParent<HealthController>();
    }

    private void Update()
    {
        HealthbarFill.fillAmount = (float)Mathf.Clamp01((float)HealthController.CurrentHealth / HealthController.MaximumHealth);
    }
}
