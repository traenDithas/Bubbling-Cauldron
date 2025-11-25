using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    // --- LIVE STATISTICS (Current Game) ---
    public int CurrentScore { get; private set; }
    public int CorrectIngredients { get; private set; }
    public int WrongIngredients { get; private set; }
    public int TrashedIngredients { get; private set; }
    public int TotalIngredientsEncountered => CorrectIngredients + WrongIngredients + TrashedIngredients;
    
    // Internal State
    private int currentStreak;
    private int highScore;
    private float startTime; // To track playtime

    void Start()
    {
        CurrentScore = 0;
        currentStreak = 1; 
        CorrectIngredients = 0;
        WrongIngredients = 0;
        TrashedIngredients = 0;
        startTime = Time.time;

        // Load existing High Score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();
    }

    public void AddScore(int baseAmount)
    {
        int totalAdded = baseAmount * currentStreak;
        CurrentScore += totalAdded;
        
        CorrectIngredients++;

        // Check for High Score immediately
        if (CurrentScore > highScore)
        {
            highScore = CurrentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        
        // Every time we score, we check if we broke other records too
        CheckAndSaveLifetimeStats();
        
        UpdateUI();
    }

    public void RecordWrongIngredient()
    {
        WrongIngredients++;
        CheckAndSaveLifetimeStats();
    }

    public void RecordTrashedIngredient()
    {
        TrashedIngredients++;
        CheckAndSaveLifetimeStats();
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

    // --- NEW: SAVE ALL LIFETIME STATS ---
    // This function checks if the current stats are better than the saved ones.
    public void CheckAndSaveLifetimeStats()
    {
        // 1. Total Ingredients Collected (Max)
        int oldMaxIngredients = PlayerPrefs.GetInt("MaxIngredients", 0);
        if (TotalIngredientsEncountered > oldMaxIngredients)
        {
            PlayerPrefs.SetInt("MaxIngredients", TotalIngredientsEncountered);
        }

        // 2. Max Correct Ingredients
        int oldMaxCorrect = PlayerPrefs.GetInt("MaxCorrect", 0);
        if (CorrectIngredients > oldMaxCorrect)
        {
            PlayerPrefs.SetInt("MaxCorrect", CorrectIngredients);
        }

        // 3. Longest Playtime (in seconds)
        float currentRunTime = Time.time - startTime;
        float oldMaxTime = PlayerPrefs.GetFloat("MaxPlaytime", 0f);
        if (currentRunTime > oldMaxTime)
        {
            PlayerPrefs.SetFloat("MaxPlaytime", currentRunTime);
        }
        
        // 4. Note: Max Level/Speed is handled in RecipeManager or can be added here if we passed the level in.
        
        PlayerPrefs.Save();
    }
}