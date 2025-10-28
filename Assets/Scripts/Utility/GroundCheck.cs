using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Transform GroundCheckTransform;
    public float GroundCheckRadius;
    public LayerMask GroundLayer;
    public bool Grounded;

    private void Update()
    {
        Grounded = IsGrounded();
    }

    private bool IsGrounded()
    {
        Collider[] hitColliders = Physics.OverlapSphere(GroundCheckTransform.position, GroundCheckRadius, GroundLayer);
        return hitColliders.Length > 0;
    }
}
