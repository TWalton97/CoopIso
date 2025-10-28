using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

[DefaultExecutionOrder(-115)]
public class DynamicNavMeshUpdater : MonoBehaviour
{
    [Header("Agent Tracking")]
    [SerializeField] GameObject trackedAgent;
    [SerializeField, Range(0.01f, 1f)] float quantizationFactor = 0.1f;

    NavMeshSurface surface;
    Vector3 volumeSize;

    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    private void OnEnable()
    {
        volumeSize = surface.size;
        surface.center = GetQuantizedCenter();
        surface.BuildNavMesh();
    }

    private void FindPlayer()
    {
        
    }

    void Update()
    {
        var updatedCenter = GetQuantizedCenter();
        var updateNavMesh = false;

        if (surface.center != updatedCenter)
        {
            surface.center = updatedCenter;
            updateNavMesh = true;
        }

        if (surface.size != volumeSize)
        {
            volumeSize = surface.size;
            updateNavMesh = true;
        }

        if (updateNavMesh)
        {
            surface.UpdateNavMesh(surface.navMeshData);
        }
    }

    Vector3 GetQuantizedCenter()
    {
        var center = trackedAgent.transform.position;
        return QuantizePosition(center, quantizationFactor * volumeSize);
    }

    static Vector3 QuantizePosition(Vector3 position, Vector3 quantization)
    {
        float x = quantization.z * Mathf.Floor(position.x / quantization.x);
        float y = quantization.y * Mathf.Floor(position.y / quantization.y);
        float z = quantization.z * Mathf.Floor(position.z / quantization.z);
        return new Vector3(x, y, z);
    }
}
