using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI streakText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("References (For Saving Stats)")]
    [SerializeField] private RecipeManager recipeManager; 
    [SerializeField] private IngredientSpawner ingredientSpawner;

    // --- LIVE STATISTICS ---
    public int CurrentScore { get; private set; }
    public int CorrectIngredients { get; private set; }
    public int WrongIngredients { get; private set; }
    public int TrashedIngredients { get; private set; }
    public int TotalIngredientsEncountered => CorrectIngredients + WrongIngredients + TrashedIngredients;
    
    // Internal State
    private int currentStreak;
    private int highScore;
    private float startTime; 

    void Start()
    {
        CurrentScore = 0;
        currentStreak = 1; 
        CorrectIngredients = 0;
        WrongIngredients = 0;
        TrashedIngredients = 0;
        startTime = Time.time;

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateUI();
    }

    public void AddScore(int baseAmount)
    {
        int totalAdded = baseAmount * currentStreak;
        CurrentScore += totalAdded;
        
        CorrectIngredients++;

        if (CurrentScore > highScore)
        {
            highScore = CurrentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
        
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

    // --- UPDATED: SAVE ALL 7 STATS ---
    public void CheckAndSaveLifetimeStats()
    {
        // 1. Total Ingredients
        int oldMaxIngredients = PlayerPrefs.GetInt("MaxIngredients", 0);
        if (TotalIngredientsEncountered > oldMaxIngredients)
            PlayerPrefs.SetInt("MaxIngredients", TotalIngredientsEncountered);

        // 2. Max Correct
        int oldMaxCorrect = PlayerPrefs.GetInt("MaxCorrect", 0);
        if (CorrectIngredients > oldMaxCorrect)
            PlayerPrefs.SetInt("MaxCorrect", CorrectIngredients);

        // 3. Max Trash (The "Messiest Run")
        int oldMaxTrash = PlayerPrefs.GetInt("MaxTrash", 0);
        if (TrashedIngredients > oldMaxTrash)
            PlayerPrefs.SetInt("MaxTrash", TrashedIngredients);

        // 4. Longest Playtime
        float currentRunTime = Time.time - startTime;
        float oldMaxTime = PlayerPrefs.GetFloat("MaxPlaytime", 0f);
        if (currentRunTime > oldMaxTime)
            PlayerPrefs.SetFloat("MaxPlaytime", currentRunTime);
        
        // 5. Most Recipes (Need RecipeManager reference)
        if (recipeManager != null)
        {
            int oldMaxRecipes = PlayerPrefs.GetInt("MaxRecipes", 0);
            if (recipeManager.RecipesCompleted > oldMaxRecipes)
                PlayerPrefs.SetInt("MaxRecipes", recipeManager.RecipesCompleted);
        }

        // 6. Highest Level (Need Spawner reference, simplified via RecipeManager count)
        // Since Level = RecipesCompleted (clamped to 9), we can use that logic or the spawner
        if (recipeManager != null)
        {
            int currentLevel = Mathf.Clamp(recipeManager.RecipesCompleted, 0, 9);
            int oldMaxLevel = PlayerPrefs.GetInt("MaxLevel", 0);
            if (currentLevel > oldMaxLevel)
                PlayerPrefs.SetInt("MaxLevel", currentLevel);
        }
        
        PlayerPrefs.Save();
    }
}