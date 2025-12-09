using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeManager : MonoBehaviour
{
    [Header("Recipe UI Icons (Fixed Slots)")]
    [Tooltip("Drag the 4 fixed icon GameObjects from the Hierarchy here.")]
    [SerializeField] private RecipeIcon[] fixedRecipeIconSlots = new RecipeIcon[4]; 

    [Header("Recipe Settings")]
    [Tooltip("How much heat (0-1) to add for each wrong ingredient.")]
    [SerializeField] private float heatPerWrongIngredient = 0.25f;
    
    [Header("Difficulty & Progression")]
    [Tooltip("The total number of recipes to complete to reach max difficulty.")]
    [SerializeField] private int recipesToMaxLevel = 10;

    [Header("Component References")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private CauldronGauge cauldronGauge;
    [SerializeField] private IngredientSpawner ingredientSpawner; 
    [SerializeField] private LevelArrow levelArrow;

    // State Tracking
    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>(); 
    private List<string> currentRecipeIDs = new List<string>();
    private int requiredFillGoal;
    
    public int RecipesCompleted { get; private set; } = 0;

    void Start()
    {
        if (ingredientSpawner == null) Debug.LogError("CRITICAL ERROR: RecipeManager is missing IngredientSpawner!");

        RecipesCompleted = 0;
        float normalizedDifficulty = 0f;
        
        if (levelArrow != null) levelArrow.SetValue(normalizedDifficulty);
        if (ingredientSpawner != null) ingredientSpawner.UpdateDifficulty(normalizedDifficulty, RecipesCompleted);
        
        GenerateNewRecipe();
    }

    public void GenerateNewRecipe()
    {
        ingredientCounts.Clear(); 
        currentRecipeIDs.Clear();
        
        // Hide all slots initially
        foreach (RecipeIcon icon in fixedRecipeIconSlots)
        {
            if(icon != null) icon.gameObject.SetActive(false); 
        }

        if (ingredientSpawner == null) return;

        requiredFillGoal = ingredientSpawner.GetCurrentLevelFillGoal(); 
        int recipeSize = ingredientSpawner.GetCurrentLevelRecipeSize();
        
        List<IngredientSpawnData> availableIngredients = ingredientSpawner.GetCurrentLevelIngredients();

        if (availableIngredients.Count == 0) return;

        // Loop through the slots we need
        for (int i = 0; i < recipeSize && i < fixedRecipeIconSlots.Length; i++)
        {
            int randomIndex = Random.Range(0, availableIngredients.Count);
            GameObject ingredientPrefab = availableIngredients[randomIndex].prefab;
            IngredientData ingredientData = ingredientPrefab.GetComponent<IngredientData>();
            
            if(ingredientData != null)
            {
                RecipeIcon iconSlot = fixedRecipeIconSlots[i];
                
                // Activate the slot
                iconSlot.gameObject.SetActive(true);
                
                // 1. Initialize Logic ID
                iconSlot.Initialize(ingredientData.ingredientID, requiredFillGoal);
                
                // 2. Initialize Visuals (Swap the TEXTURE, not the Material)
                if (ingredientData.iconTexture != null)
                {
                    iconSlot.SetVisual(ingredientData.iconTexture);
                }
                else
                {
                    Debug.LogWarning("Ingredient " + ingredientData.ingredientID + " is missing its Icon Texture!");
                }
                
                if(!ingredientCounts.ContainsKey(ingredientData.ingredientID))
                {
                    ingredientCounts.Add(ingredientData.ingredientID, 0);
                    currentRecipeIDs.Add(ingredientData.ingredientID);
                }
            }
        }

        if (cauldronGauge != null) cauldronGauge.SetHeat(0f);
        if (ingredientSpawner != null) ingredientSpawner.StartSpawning();
    }

    public void OnIngredientCaught(IngredientData caughtIngredient)
    {
        if (ingredientCounts.ContainsKey(caughtIngredient.ingredientID))
        {
            if (ingredientCounts[caughtIngredient.ingredientID] < requiredFillGoal)
            {
                // SUCCESS
                ingredientCounts[caughtIngredient.ingredientID]++;
                
                // Find and update the correct icon
                foreach(var icon in fixedRecipeIconSlots)
                {
                    if (icon.gameObject.activeSelf && icon.ingredientID == caughtIngredient.ingredientID)
                    {
                        icon.UpdateFill(ingredientCounts[caughtIngredient.ingredientID]);
                        break;
                    }
                }

                scoreManager.IncrementStreak();
                scoreManager.AddScore(caughtIngredient.scoreValue);
                CheckForRecipeCompletion();
            }
            else
            {
                HandleWrongIngredient();
            }
        }
        else
        {
            HandleWrongIngredient();
        }
    }

    private void HandleWrongIngredient()
    {
        scoreManager.RecordWrongIngredient(); 
        scoreManager.ResetStreak();
        if (cauldronGauge != null) cauldronGauge.AddHeat(heatPerWrongIngredient);
    }

    private void CheckForRecipeCompletion()
    {
        bool isComplete = true;
        foreach (var kvp in ingredientCounts)
        {
            if (kvp.Value < requiredFillGoal)
            {
                isComplete = false;
                break;
            }
        }

        if (isComplete)
        {
            HandleRecipeComplete();
            Invoke("GenerateNewRecipe", 1.5f);
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
        return currentRecipeIDs.Contains(idToCheck);
    }
}