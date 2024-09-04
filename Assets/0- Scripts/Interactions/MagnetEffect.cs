using UnityEngine;

public class MagnetEffect : MonoBehaviour
{
    public Transform player;             // Reference to the player's transform
    public float detectionRadius = 5f;   // Radius within which the sphere will start being attracted
    public float attractionForce = 10f;  // Force or speed of attraction
    public float stopDistance = 1f;      // Distance at which the sphere will stop moving towards the player

    private Rigidbody rb;                // Rigidbody of the sphere

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < detectionRadius && distanceToPlayer > stopDistance)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            rb.MovePosition(transform.position + directionToPlayer * (attractionForce * Time.fixedDeltaTime));
          
        }
        transform.localScale = Vector3.one;
        transform.parent.transform.localScale = Vector3.one;
    }
}