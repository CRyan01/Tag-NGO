using Unity.Netcode;
using UnityEngine;

// Server authoratative, handles tag + scoring rules.
// Tracks who is It, and gives points to whoever is not It each tick.
// Uses a cooldown between tags.
// Handles 2 players.
public class TagManager : NetworkBehaviour {
    // To track who is It (0 if not It, 1 if It).
    public NetworkVariable<int> ItIndex = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );

    // To track players scores.
    public NetworkVariable<int> playerOneScore = new NetworkVariable<int>(0);
    public NetworkVariable<int> playerTwoScore = new NetworkVariable<int>(0);

    public float scoreTickSeconds = 1.0f; // how often points are awarded.
    public float tagCooldownSeconds = 1.0f; // min time between tags.

    public int pointsToWin = 120; // points needed to win.

    float nextTick; // next time to grant score.
    float nextTagTime; // next time a tag is allowed.

    int nextPlayerIndex = 0; // used to assign indexes 0 and then 1 when players join.

    public override void OnNetworkSpawn() {
        // Only the server should run game logic.
        if (!IsServer) {
            return;
        }

        // Initialize timers.
        nextTick = Time.time + scoreTickSeconds;
        nextTagTime = Time.time;

        // Listen for players joining.
        NetworkManager.OnClientConnectedCallback += OnClientConnected;

        // Handle the host already being connected.
        foreach (var id in NetworkManager.ConnectedClientsIds) {
            AssignIndexIfNeeded(id);
        }
    }

    private void OnDestroy() {
        // Cleanup callbacks.
        if (NetworkManager != null) {
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    // Called by NGO if a client connects.
    private void OnClientConnected(ulong clientId) {
        AssignIndexIfNeeded(clientId);

        // When both players are there, randomly pick who starts as It.
        if (NetworkManager.ConnectedClientsIds.Count >= 2) {
            ItIndex.Value = Random.Range(0, 2);
        }
    }

    // Assign a player index (0 or 1) if not already assigned.
    private void AssignIndexIfNeeded(ulong clientId) {
        // Check if both players have already been assigned.
        if (nextPlayerIndex > 1) {
            return;
        }

        // Get the player object for this client.
        var playerObject = NetworkManager.ConnectedClients[clientId].PlayerObject;
        if (playerObject == null) {
            return;
        }

        // Get the player state for this client.
        var playerState = playerObject.GetComponent<PlayerState>();
        if (playerState == null) {
            return;
        }

        // Dont assign an index more than once.
        if (playerState.Index.Value != -1) {
            return;
        }

        // Assign index 0 first, then 1.
        playerState.Index.Value = nextPlayerIndex;
        nextPlayerIndex++;
    }

    private void Update() {
        // Only the server updates scores.
        if (!IsServer) {
            return;
        }

        // Award points every tick.
        if (Time.time >= nextTick && !IsGameOver()) {
            nextTick = Time.time + scoreTickSeconds;

            // Give points to the player who is not It.
            int notIt;
            if (ItIndex.Value == 0) {
                notIt = 1;
            } else {
                notIt = 0;
            }

            AddPoint(notIt);
        }
    }

    // Called by PlayerTag when a collision occurs.
    public void TryTagServer(int taggerIndex, int taggedIndex) {
        // Only the server processes tags.
        if (!IsServer) {
            return;
        }

        // Ensure the game is still running.
        if (IsGameOver()) {
            return;
        }

        // Enforce the tag cooldown.
        if (Time.time < nextTagTime) {
            return;
        }

        // Only the player who is It can tag.
        if (taggerIndex != ItIndex.Value) {
            return;
        }

        // Ignore invalid or self tags.
        if (taggedIndex == ItIndex.Value) {
            return;
        }

        // Swap who is It.
        ItIndex.Value = taggedIndex;
        nextTagTime = Time.time + tagCooldownSeconds;

        // Reward a successful tag with a point.
        AddPoint(taggerIndex);
    }

    // Add a point to the specified player.
    private void AddPoint(int index) {
        if (index == 0) {
            playerOneScore.Value++;
        } else {
            playerTwoScore.Value++;
        }
    }

    // Check if either player has won.
    private bool IsGameOver() {
        // If either player has reached pointsToWin return true.
        if (playerOneScore.Value >= pointsToWin || playerTwoScore.Value >= pointsToWin) {
            return true;
        }

        return false; // Otherwise return false.
    }
}
