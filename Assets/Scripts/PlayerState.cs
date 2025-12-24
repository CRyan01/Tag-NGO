using Unity.Netcode;

// Stores per-player network state, -1 means unassigned.
public class PlayerState : NetworkBehaviour {
    public NetworkVariable<int> Index = new NetworkVariable<int>(
        -1,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );
}
