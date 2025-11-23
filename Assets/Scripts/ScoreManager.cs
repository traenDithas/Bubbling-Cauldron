using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    // --- STATISTICS ---
    // We use "properties" { get; private set; } so other scripts can READ them
    // but only this script can CHANGE them.
    public int CurrentScore { get; private set; }
    public int CorrectIngredients { get; private set; }
    public int WrongIngredients { get; private set; }
    public int TrashedIngredients { get; private set; }
    
    // Helper to get total collected (Correct + Wrong + Trashed)
    public int TotalIngredientsEncountered => CorrectIngredients + WrongIngredients + TrashedIngredients;

    // Internal State
    private int currentStreak;
    private int highScore;

    void Start()
    {
        CurrentScore = 0;
        currentStreak = 1; 
        CorrectIngredients = 0;
        WrongIngredients = 0;
        TrashedIngredients = 0;

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();
    }

    public void AddScore(int baseAmount)
    {
        int totalAdded = baseAmount * currentStreak;
        CurrentScore += totalAdded;
        
        // Track Statistic
        CorrectIngredients++;

        if (CurrentScore > highScore)
        {
            highScore = CurrentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        UpdateUI();
    }

    // --- NEW: Track Wrong Items ---
    public void RecordWrongIngredient()
    {
        WrongIngredients++;
        // Wrong ingredients usually break streak, handled in RecipeManager
    }

    // --- NEW: Track Trashed Items ---
    public void RecordTrashedIngredient()
    {
        TrashedIngredients++;
    }

    public void IncrementStreak()
    {
        currentStreak++;
        UpdateUI();
    }

    public void ResetStreak()
    {
        currentStreak = 1;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = CurrentScore.ToString();
        if (streakText != null) streakText.text = "x" + currentStreak.ToString();
        if (highScoreText != null) highScoreText.text = "Best: " + highScore.ToString();
    }
}