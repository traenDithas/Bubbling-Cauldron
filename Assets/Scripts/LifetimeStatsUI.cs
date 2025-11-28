using UnityEngine;
using TMPro;

public class LifetimeStatsUI : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject statsPanel;

    [Header("Text Fields (For Lifetime Records)")]
    [SerializeField] private TextMeshProUGUI textBestScore;
    [SerializeField] private TextMeshProUGUI textLongestTime;
    
    // --- NEW FIELDS ---
    [SerializeField] private TextMeshProUGUI textMostRecipes;
    [SerializeField] private TextMeshProUGUI textMostIngredients;
    [SerializeField] private TextMeshProUGUI textMostCorrect;
    [SerializeField] private TextMeshProUGUI textMostTrash;
    [SerializeField] private TextMeshProUGUI textHighestLevel;
    // ------------------

    private bool isVisible = false;

    void Start()
    {
        if (statsPanel != null) statsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleStats();
        }
    }

    public void ToggleStats()
    {
        isVisible = !isVisible;

        if (isVisible)
        {
            LoadAndDisplayStats();
            if (statsPanel != null) statsPanel.SetActive(true);
        }
        else
        {
            if (statsPanel != null) statsPanel.SetActive(false);
        }
    }

    private void LoadAndDisplayStats()
    {
        // 1. High Score
        if (textBestScore != null) 
            textBestScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();

        // 2. Longest Time
        float maxTime = PlayerPrefs.GetFloat("MaxPlaytime", 0f);
        int minutes = Mathf.FloorToInt(maxTime / 60);
        int seconds = Mathf.FloorToInt(maxTime % 60);
        if (textLongestTime != null) 
            textLongestTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // 3. Most Recipes
        if (textMostRecipes != null)
            textMostRecipes.text = PlayerPrefs.GetInt("MaxRecipes", 0).ToString();

        // 4. Most Ingredients (Total)
        if (textMostIngredients != null) 
            textMostIngredients.text = PlayerPrefs.GetInt("MaxIngredients", 0).ToString();

        // 5. Most Correct
        if (textMostCorrect != null) 
            textMostCorrect.text = PlayerPrefs.GetInt("MaxCorrect", 0).ToString();
            
        // 6. Most Trash
        if (textMostTrash != null) 
            textMostTrash.text = PlayerPrefs.GetInt("MaxTrash", 0).ToString();
            
        // 7. Highest Level
        if (textHighestLevel != null) 
            textHighestLevel.text = PlayerPrefs.GetInt("MaxLevel", 0).ToString();
    }
    
    [ContextMenu("Reset Lifetime Stats")]
    public void ResetStats()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Lifetime stats deleted.");
    }
}