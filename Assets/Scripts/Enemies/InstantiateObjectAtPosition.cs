using UnityEngine;

public class InstantiateObjectAtPosition : MonoBehaviour
{
    public GameObject objToInstantiate;
    public Transform spawnPos;
    public float objDuration;
    private GameObject spawnedObj;
    public bool CopyRotation = false;

    public void InstantiateObj()
    {
        if (CopyRotation)
        {
            spawnedObj = Instantiate(objToInstantiate, spawnPos.position, spawnPos.rotation);
        }
        else
        {
            spawnedObj = Instantiate(objToInstantiate, spawnPos.position, Quaternion.identity);
        }
        
        Destroy(spawnedObj, objDuration);
    }
}
