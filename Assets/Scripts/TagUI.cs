using UnityEngine;
using TMPro;

public class TagUI : MonoBehaviour {
    public TMP_Text scoreText; // text to display current score.
    public TMP_Text itText; // text to display whos It.
    public TMP_Text winText; // text to display the winner when a match ends.

    TagManager tagManager; // to store a ref to TagManager.

    private void Start() {
        // Get and store a ref to TagManager.
        tagManager = FindFirstObjectByType<TagManager>();
    }

    private void Update() {
        // Check for a valid ref to TagManager.
        if (tagManager == null) {
            return;
        }

        // Update score text.
        if (scoreText != null) {
            scoreText.text = "Player 1: " + tagManager.playerOneScore.Value + "   Player 2: " + tagManager.playerTwoScore.Value;
        }

        // Update It text.
        if (itText != null) {
            itText.text = "It: Player " + (tagManager.ItIndex.Value + 1);
        }

        // Display win text when game over conditions are met.
        if (winText != null) {
            if (tagManager.playerOneScore.Value >= tagManager.pointsToWin) {
                // If player 1 wins.
                winText.text = "Player 1 Wins!";
            } else if (tagManager.playerTwoScore.Value >= tagManager.pointsToWin) {
                // If player 2 wins.
                winText.text = "Player 2 Wins!";
            } else {
                // Otherwise display an empty string.
                winText.text = "";
            }
        }
    }
}