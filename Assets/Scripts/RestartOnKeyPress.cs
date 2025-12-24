using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// Restart the game when R is pressed.
public class RestartOnKeyPress : MonoBehaviour {
    private void Update() {
        // Only the server and host should be able to restart.
        if (NetworkManager.Singleton == null) {
            return;
        }
        if (!NetworkManager.Singleton.IsServer) {
            return;
        }

        // When R is pressed.
        if (Input.GetKeyDown(KeyCode.R)) {
            // Reload the scene for everyone.
            NetworkManager.Singleton.SceneManager.LoadScene(
                SceneManager.GetActiveScene().name,
                LoadSceneMode.Single
                );
        }
    }
}
