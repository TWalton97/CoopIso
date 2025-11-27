using UnityEngine;
using System;

public class EntityIdentity : MonoBehaviour
{
    [SerializeField]
    private string guid;

    public string GUID => guid;
}
