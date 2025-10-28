using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField] private List<Transform> targetObjects;

    [SerializeField] private LayerMask wallMask;

    private Camera mainCamera;


    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Start()
    {
        PlayerJoinManager.OnPlayerJoinedEvent += AddPlayer;
    }

    void OnDisable()
    {
        PlayerJoinManager.OnPlayerJoinedEvent -= AddPlayer;
    }

    private void AddPlayer(GameObject obj)
    {
        targetObjects.Add(obj.transform);
    }

    private void Update()
    {
        foreach (Transform targetObject in targetObjects)
        {
            Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
            cutoutPos.y /= Screen.width / Screen.height;

            Vector3 offset = targetObject.position - transform.position;
            RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

            for (int i = 0; i < hitObjects.Length; ++i)
            {
                Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

                for (int m = 0; m < materials.Length; ++m)
                {
                    materials[m].SetVector("_CutoutPos", cutoutPos);
                    materials[m].SetFloat("_CutoutSize", 0.15f);
                    materials[m].SetFloat("_FalloffSize", 0.02f);
                }
            }
        }
    }
}
