using Unity.Netcode;
using UnityEngine;

// Detects player collisions on the server, and calls uses tag manager to process
// a tag.
public class PlayerTag : NetworkBehaviour {
    PlayerState thisPlayer;
    TagManager tagManager;

    private void Awake() {
        // Store a ref to our PlayerState.
        thisPlayer = GetComponent<PlayerState>();
    }

    public override void OnNetworkSpawn() {
        // Find and store a ref to TagManager.
        tagManager = FindFirstObjectByType<TagManager>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Collision detected: " + gameObject.name + " IsServer:" + IsServer);

        // Only the server needs to check if the tag is valid.
        if (!IsServer) {
            return;
        }

        // Check for a valid TagManager ref.
        if (tagManager == null) {
            return;
        }

        // Check if the other object is a player, and get a ref to its PlayerState.
        var otherPlayer = other.GetComponentInParent<PlayerState>();
        if (otherPlayer == null) {
            return;
        }

        // Ensure only valid indexes are passed to TagManager (0, and 1).
        if (thisPlayer.Index.Value == -1 || otherPlayer.Index.Value == -1) {
            return;
        }

        // Ask the server to process the tag.
        tagManager.TryTagServer(thisPlayer.Index.Value, otherPlayer.Index.Value);
    }
}
