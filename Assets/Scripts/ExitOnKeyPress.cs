using UnityEngine;

// Quit the game when Esc is pressed.
public class ExitOnKeyPress : MonoBehaviour {
    private void Update() {
        // When escape is pressed.
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Quit the game.
            Application.Quit();

#if UNITY_EDITOR
            // Quit the game in editor.
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
