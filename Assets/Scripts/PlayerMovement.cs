using Unity.Netcode;
using UnityEngine;

// Handles player movement.
public class PlayerMovement : NetworkBehaviour {

    public float speed = 5.0f; // how fast the player moves?

    private BoxCollider2D arenaBounds; // the bounds of the arena.

    private Rigidbody2D rb; // to store a reference to the players rb component.
    private Vector2 move; // to store the players movement vector.

    private void Awake() {
        // Get a reference to the players rb component.
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        // Get a ref to the arenas box collider.
        GameObject arena = GameObject.FindGameObjectWithTag("ArenaBounds");
        if (arena != null) {
            arenaBounds = arena.GetComponent<BoxCollider2D>();
        }
    }

    private void Update() {
        // Check ownership before updating.
        if (!IsOwner) {
            return;
        }

        // Get raw movement input.
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        // Update the movement vector.
        move = move.normalized;
    }

    private void FixedUpdate() {
        // Check ownership before updating.
        if (!IsOwner) {
            return;
        }

        // Calculate the move target.
        Vector2 target = rb.position + move * speed * Time.fixedDeltaTime;

        // Keep the player within arena bounds.
        if (arenaBounds != null) {
            Bounds bounds = arenaBounds.bounds; // get the arenas bounds.

            float playerRadius = 0.5f; // subtract half the players size to prevent clipping.

            // Clamp movement within bounds.
            target.x = Mathf.Clamp(target.x, bounds.min.x + playerRadius, bounds.max.x - playerRadius);
            target.y = Mathf.Clamp(target.y, bounds.min.y + playerRadius, bounds.max.y - playerRadius);
        }

        // Apply the movement.
        rb.MovePosition(target);
    }
}
