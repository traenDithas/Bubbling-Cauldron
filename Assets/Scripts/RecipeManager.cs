using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeManager : MonoBehaviour
{
    [Header("Recipe Settings")]
    [SerializeField] private float heatPerWrongIngredient = 0.25f;
    
    [Header("Difficulty & Progression")]
    [SerializeField] private int recipesToMaxLevel = 10;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI recipeText;

    [Header("Component References")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private CauldronGauge cauldronGauge;
    [SerializeField] private IngredientSpawner ingredientSpawner; 
    [SerializeField] private LevelArrow levelArrow;

    private List<IngredientData> currentRecipe = new List<IngredientData>();
    
    // --- STATISTICS ---
    // Public getter so PauseManager can read it
    public int RecipesCompleted { get; private set; } = 0;

    void Start()
    {
        if (ingredientSpawner == null) Debug.LogError("Missing IngredientSpawner!");

        RecipesCompleted = 0;
        float normalizedDifficulty = 0f;
        
        if (levelArrow != null) levelArrow.SetValue(normalizedDifficulty);
        if (ingredientSpawner != null) ingredientSpawner.UpdateDifficulty(normalizedDifficulty, RecipesCompleted);
        
        GenerateNewRecipe();
    }

    public void GenerateNewRecipe()
    {
        currentRecipe.Clear();

        if (ingredientSpawner == null) return;

        List<IngredientSpawnData> availableIngredients = ingredientSpawner.GetCurrentLevelIngredients();
        if (availableIngredients.Count == 0) return;

        int currentLevelSize = ingredientSpawner.GetCurrentLevelRecipeSize();
        
        for (int i = 0; i < currentLevelSize; i++)
        {
            int randomIndex = Random.Range(0, availableIngredients.Count);
            GameObject ingredientPrefab = availableIngredients[randomIndex].prefab;
            IngredientData ingredientData = ingredientPrefab.GetComponent<IngredientData>();
            
            if(ingredientData != null) currentRecipe.Add(ingredientData);
        }

        UpdateRecipeUI();
        if (cauldronGauge != null) cauldronGauge.SetHeat(0f);
        if (ingredientSpawner != null) ingredientSpawner.StartSpawning();
    }

    private void UpdateRecipeUI()
    {
        string recipeString = "Rezp:\n"; 
        if (currentRecipe.Count == 0) recipeString += "Fertig!";
        else
        {
            foreach (IngredientData ingredient in currentRecipe) recipeString += "- " + ingredient.ingredientID + "\n";
        }
        recipeText.text = recipeString;
    }

    public void OnIngredientCaught(IngredientData caughtIngredient)
    {
        bool wasInRecipe = false;
        for (int i = currentRecipe.Count - 1; i >= 0; i--)
        {
            if (currentRecipe[i].ingredientID == caughtIngredient.ingredientID)
            {
                wasInRecipe = true;
                scoreManager.IncrementStreak();
                scoreManager.AddScore(caughtIngredient.scoreValue); // This increments "Correct" stat
                currentRecipe.RemoveAt(i);
                break;
            }
        }

        if (wasInRecipe)
        {
            UpdateRecipeUI();
            if (currentRecipe.Count == 0)
            {
                HandleRecipeComplete(); 
                Invoke("GenerateNewRecipe", 1.5f);
            }
        }
        else
        {
            // --- STAT TRACKING ---
            scoreManager.RecordWrongIngredient(); 
            // --------------------
            
            scoreManager.ResetStreak();
            if (cauldronGauge != null) cauldronGauge.AddHeat(heatPerWrongIngredient);
        }
    }

    private void HandleRecipeComplete()
    {
        if (ingredientSpawner != null) ingredientSpawner.StopSpawning();

        RecipesCompleted++;
        
        float normalizedDifficulty = (recipesToMaxLevel > 0) ? (float)RecipesCompleted / (float)recipesToMaxLevel : 0f;
        normalizedDifficulty = Mathf.Clamp01(normalizedDifficulty); 

        if (levelArrow != null) levelArrow.SetValue(normalizedDifficulty);
        if (ingredientSpawner != null) ingredientSpawner.UpdateDifficulty(normalizedDifficulty, RecipesCompleted);
    }

    public bool IsIngredientInCurrentRecipe(string idToCheck)
    {
        foreach (IngredientData item in currentRecipe)
        {
            if (item.ingredientID == idToCheck) return true; 
        }
        return false;
    }
}