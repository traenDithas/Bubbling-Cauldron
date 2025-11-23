using UnityEngine;
using TMPro; // Needed for TextMeshPro

public class PauseManager : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject pauseMenuUI;

    [Header("Stats Text Fields")]
    [Tooltip("Drag text_Score here")]
    [SerializeField] private TextMeshProUGUI textScore;
    
    [Tooltip("Drag text_Time here")]
    [SerializeField] private TextMeshProUGUI textTime;
    
    [Tooltip("Drag text_Recipe here")]
    [SerializeField] private TextMeshProUGUI textRecipeCount;
    
    [Tooltip("Drag text_Ingredients here (Total collected)")]
    [SerializeField] private TextMeshProUGUI textTotalIngredients;
    
    [Tooltip("Drag text_Correct here")]
    [SerializeField] private TextMeshProUGUI textCorrectIngredients;
    
    [Tooltip("Drag text_Trash here")]
    [SerializeField] private TextMeshProUGUI textTrashedIngredients;
    
    [Tooltip("Drag text_Level here (Speed)")]
    [SerializeField] private TextMeshProUGUI textSpeedLevel;

    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private IngredientSpawner ingredientSpawner; // To get the exact level index

    private bool isPaused = false;
    private float startTime;

    void Start()
    {
        // Track when the game actually started
        startTime = Time.time;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            UpdatePauseStats(); // Update text before showing it
            Time.timeScale = 0f; 
            if(pauseMenuUI != null) pauseMenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f; 
            if(pauseMenuUI != null) pauseMenuUI.SetActive(false);
        }
    }

    private void UpdatePauseStats()
    {
        // 1. Playtime (Minutes)
        float timePlayed = Time.time - startTime;
        int minutes = Mathf.FloorToInt(timePlayed / 60);
        int seconds = Mathf.FloorToInt(timePlayed % 60);
        if (textTime != null) textTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // 2. Score
        if (scoreManager != null && textScore != null) 
            textScore.text = scoreManager.CurrentScore.ToString();

        // 3. Recipes Completed
        if (recipeManager != null && textRecipeCount != null) 
            textRecipeCount.text = recipeManager.RecipesCompleted.ToString();

        // 4. Ingredients (Total, Correct, Trash)
        if (scoreManager != null)
        {
            // Total = Correct + Wrong + Trashed
            int total = scoreManager.CorrectIngredients + scoreManager.WrongIngredients + scoreManager.TrashedIngredients;

            if (textTotalIngredients != null) textTotalIngredients.text = total.ToString();
            if (textCorrectIngredients != null) textCorrectIngredients.text = scoreManager.CorrectIngredients.ToString();
            if (textTrashedIngredients != null) textTrashedIngredients.text = scoreManager.TrashedIngredients.ToString();
        }

        // 5. Speed (Level 0-9)
        // We need to ask the spawner for the current Level Index
        int currentLevel = 0;
        if (ingredientSpawner != null)
        {
            // Access the private index via a public property? 
            // Or simpler: Calculate it from recipes completed like we do in update difficulty
            // Let's rely on RecipeManager count for now as it's cleaner
            currentLevel = recipeManager != null ? recipeManager.RecipesCompleted : 0;
            // Clamp it to 9 if you have 9 levels
            if (currentLevel > 9) currentLevel = 9; 
        }
        
        if (textSpeedLevel != null) textSpeedLevel.text = currentLevel.ToString();
    }
}