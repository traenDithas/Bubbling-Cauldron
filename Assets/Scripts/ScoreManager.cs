using UnityEngine;
using TMPro;

/// <summary>
/// Manages the player's score AND their current winning streak.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The UI element for the total score.")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Tooltip("The UI element for the current streak (e.g. 'x5').")]
    [SerializeField] private TextMeshProUGUI streakText;

    // State
    private int currentScore;
    private int currentStreak;

    void Start()
    {
        currentScore = 0;
        currentStreak = 1; // Start with a 1x multiplier
        UpdateUI();
    }

    /// <summary>
    /// Adds points to the score, multiplied by the current streak.
    /// </summary>
    /// <param name="baseAmount">The ingredient's base value.</param>
    public void AddScore(int baseAmount)
    {
        // Calculate score based on streak
        int totalAdded = baseAmount * currentStreak;
        currentScore += totalAdded;
        
        Debug.Log("Added " + totalAdded + " points (Base: " + baseAmount + " x Streak: " + currentStreak + ")");
        
        UpdateUI();
    }

    /// <summary>
    /// Increases the streak multiplier by 1.
    /// </summary>
    public void IncrementStreak()
    {
        currentStreak++;
        UpdateUI();
        // Ideally, play a "ding" sound here later!
    }

    /// <summary>
    /// Resets the streak back to 1.
    /// </summary>
    public void ResetStreak()
    {
        if (currentStreak > 1)
        {
            Debug.Log("Streak Broken! Reset to x1.");
            // Here you could play a "failure" sound
        }
        currentStreak = 1;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }

        if (streakText != null)
        {
            // Display as "x1", "x2", etc.
            streakText.text = "x" + currentStreak.ToString();
        }
    }
}