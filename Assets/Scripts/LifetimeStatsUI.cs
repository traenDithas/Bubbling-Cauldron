using UnityEngine;
using TMPro;

public class LifetimeStatsUI : MonoBehaviour
{
    [Header("UI Panel")]
    [Tooltip("The Panel that holds the lifetime stats.")]
    [SerializeField] private GameObject statsPanel;

    [Header("Text Fields (For Lifetime Records)")]
    [SerializeField] private TextMeshProUGUI textBestScore;
    [SerializeField] private TextMeshProUGUI textLongestTime;
    [SerializeField] private TextMeshProUGUI textMostIngredients;
    [SerializeField] private TextMeshProUGUI textMostCorrect;

    private bool isVisible = false;

    void Start()
    {
        // Ensure it starts hidden
        if (statsPanel != null) statsPanel.SetActive(false);
    }

    void Update()
    {
        // Toggle with TAB key
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
        int bestScore = PlayerPrefs.GetInt("HighScore", 0);
        if (textBestScore != null) textBestScore.text = bestScore.ToString();

        // 2. Longest Time
        float maxTime = PlayerPrefs.GetFloat("MaxPlaytime", 0f);
        int minutes = Mathf.FloorToInt(maxTime / 60);
        int seconds = Mathf.FloorToInt(maxTime % 60);
        if (textLongestTime != null) textLongestTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // 3. Most Ingredients (Total)
        int maxIng = PlayerPrefs.GetInt("MaxIngredients", 0);
        if (textMostIngredients != null) textMostIngredients.text = maxIng.ToString();

        // 4. Most Correct
        int maxCorrect = PlayerPrefs.GetInt("MaxCorrect", 0);
        if (textMostCorrect != null) textMostCorrect.text = maxCorrect.ToString();
    }
    
    // Debug tool to reset stats
    [ContextMenu("Reset Lifetime Stats")]
    public void ResetStats()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Lifetime stats deleted.");
    }
}