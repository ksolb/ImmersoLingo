using UnityEngine;

// respawns the ball for maximum engagement
[RequireComponent(typeof(Rigidbody))]
public class RespawnOnFall : MonoBehaviour
{
    [SerializeField] private float fallHeight = -5f;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        if (transform.position.y < fallHeight)
            Respawn();
    }

    public void Respawn()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(startPosition, startRotation);
    }
}