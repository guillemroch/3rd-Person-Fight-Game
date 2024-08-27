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
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // If the player reference isn't set in the Inspector, find the player automatically by tag
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void FixedUpdate()
    {
        // Calculate distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within the detection radius and the sphere isn't too close
        if (distanceToPlayer < detectionRadius && distanceToPlayer > stopDistance)
        {
            // Calculate direction towards the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            // Apply attraction force (modify position or velocity)
            rb.MovePosition(transform.position + directionToPlayer * attractionForce * Time.fixedDeltaTime);
          
        }
          transform.localScale = Vector3.one;
          transform.parent.transform.localScale = Vector3.one;
    }
}