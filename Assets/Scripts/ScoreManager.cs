using UnityEngine;
using TMPro; // Import the TextMeshPro library!

/// <summary>
/// Manages the player's score and updates the score UI text.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The TextMeshPro UI element that displays the score.")]
    [SerializeField] 
    private TextMeshProUGUI scoreText;

    // The current score, kept private to protect it.
    private int currentScore;

    /// <summary>
    /// This method is called when the game starts.
    /// </summary>
    void Start()
    {
        // Initialize the score to 0.
        currentScore = 0;
        UpdateScoreText();
    }

    /// <summary>
    /// Adds a specified amount to the current score and updates the UI.
    /// </summary>
    /// <param name="amountToAdd">The number of points to add.</param>
    public void AddScore(int amountToAdd)
    {
        currentScore += amountToAdd;
        UpdateScoreText();
    }

    /// <summary>
    /// Updates the on-screen text to display the current score.
    /// </summary>
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            // Update the text. We use .ToString() to convert the number to text.
            scoreText.text = currentScore.ToString();
        }
        else
        {
            Debug.LogError("ScoreText is not assigned in the ScoreManager!");
        }
    }
}