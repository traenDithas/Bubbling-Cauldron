using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeManager : MonoBehaviour
{
    [Header("Recipe Settings")]
    [Tooltip("The number of ingredients in one recipe.")]
    [SerializeField] 
    private int recipeLength = 4;

    [Tooltip("How much heat (0-1) to add for each wrong ingredient.")]
    [SerializeField]
    private float heatPerWrongIngredient = 0.25f;
    
    [Header("Difficulty & Progression")]
    [Tooltip("The total number of recipes to complete to reach max difficulty.")]
    [SerializeField]
    private int recipesToMaxLevel = 10;

    [Header("UI Elements")]
    [SerializeField] 
    private TextMeshProUGUI recipeText;

    [Header("Component References")]
    [SerializeField] 
    private ScoreManager scoreManager;
    
    [SerializeField]
    private CauldronGauge cauldronGauge;
    
    [Tooltip("Reference to the IngredientSpawner. This is CRITICAL!")]
    [SerializeField]
    private IngredientSpawner ingredientSpawner; 
    
    [SerializeField]
    private LevelArrow levelArrow;

    private List<IngredientData> currentRecipe = new List<IngredientData>();
    private int recipesCompleted = 0;


    // --- THIS IS THE FIXED START METHOD ---
    void Start()
    {
        if (ingredientSpawner == null)
        {
            Debug.LogError("CRITICAL ERROR: RecipeManager is missing its reference to the IngredientSpawner!");
        }

        // 1. Set the initial state (level 0)
        recipesCompleted = 0;
        float normalizedDifficulty = 0f;
        
        // 2. Tell the spawner and arrow about level 0
        if (levelArrow != null)
        {
            levelArrow.SetValue(normalizedDifficulty);
        }
        if (ingredientSpawner != null)
        {
            // Because the spawner's Awake() has run, this is now safe
            ingredientSpawner.UpdateDifficulty(normalizedDifficulty, recipesCompleted);
        }
        
        // 3. NOW, generate the first recipe (which will start the spawner)
        GenerateNewRecipe();
    }
    // --- END OF FIX ---

    public void GenerateNewRecipe()
    {
        currentRecipe.Clear();

        if (ingredientSpawner == null)
        {
            Debug.LogError("IngredientSpawner not assigned in RecipeManager!");
            UpdateRecipeUI();
            return;
        }

        List<IngredientSpawnData> availableIngredients = ingredientSpawner.GetCurrentLevelIngredients();

        if (availableIngredients.Count == 0)
        {
            Debug.LogWarning("No ingredients available for this level! Spawner list is empty.");
            UpdateRecipeUI();
            return;
        }

        for (int i = 0; i < recipeLength; i++)
        {
            int randomIndex = Random.Range(0, availableIngredients.Count);
            GameObject ingredientPrefab = availableIngredients[randomIndex].prefab;
            IngredientData ingredientData = ingredientPrefab.GetComponent<IngredientData>();
            
            if(ingredientData != null)
            {
                currentRecipe.Add(ingredientData);
            }
            else
            {
                Debug.LogError("Prefab " + ingredientPrefab.name + " is missing its IngredientData component!");
            }
        }

        UpdateRecipeUI();
        
        if (cauldronGauge != null)
        {
            cauldronGauge.SetHeat(0f);
        }

        if (ingredientSpawner != null)
        {
            ingredientSpawner.StartSpawning();
        }
    }

    private void UpdateRecipeUI()
    {
        string recipeString = "Rezp:\n"; // Changed "Rezept" to "Rezp"
        if (currentRecipe.Count == 0)
        {
            recipeString += "Fertig!";
        }
        else
        {
            foreach (IngredientData ingredient in currentRecipe)
            {
                recipeString += "- " + ingredient.ingredientID + "\n";
            }
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
                scoreManager.AddScore(caughtIngredient.scoreValue);
                currentRecipe.RemoveAt(i);
                break;
            }
        }

        if (wasInRecipe)
        {
            Debug.Log("Correct ingredient: " + caughtIngredient.ingredientID);
            UpdateRecipeUI();

            if (currentRecipe.Count == 0)
            {
                Debug.Log("Recipe Complete! Generating new one.");
                HandleRecipeComplete(); // Simplified call
                Invoke("GenerateNewRecipe", 1.5f);
            }
        }
        else
        {
            Debug.Log("Wrong ingredient: " + caughtIngredient.ingredientID);
            if (cauldronGauge != null)
            {
                cauldronGauge.AddHeat(heatPerWrongIngredient);
            }
        }
    }

    // --- THIS METHOD IS NOW SIMPLIFIED ---
    private void HandleRecipeComplete()
    {
        if (ingredientSpawner != null)
        {
            ingredientSpawner.StopSpawning();
        }

        recipesCompleted++; // Just increment it
        
        // Add a small check to prevent division by zero
        float normalizedDifficulty = (recipesToMaxLevel > 0) ? (float)recipesCompleted / (float)recipesToMaxLevel : 0f;
        normalizedDifficulty = Mathf.Clamp01(normalizedDifficulty); 

        if (levelArrow != null)
        {
            levelArrow.SetValue(normalizedDifficulty);
        }

        if (ingredientSpawner != null)
        {
            // Pass both values to the spawner
            ingredientSpawner.UpdateDifficulty(normalizedDifficulty, recipesCompleted);
        }
    }
}