using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeManager : MonoBehaviour
{
    [Header("Difficulty & Progression")]
    [SerializeField]
    private int recipesToMaxLevel = 10;

    [Tooltip("The total number of recipes to complete to reach max difficulty.")]
    [SerializeField]
    // This tracks our current progress
    private int recipesCompleted = 0;

    [SerializeField]
    private LevelArrow levelArrow;

    [SerializeField]
    private IngredientSpawner ingredientSpawner;

    [Header("Recipe Settings")]
    [SerializeField] 
    private IngredientData[] allPossibleIngredients;

    [SerializeField] 
    private int recipeLength = 4;

    [Tooltip("How much heat (0-1) to add for each wrong ingredient.")]
    [SerializeField]
    private float heatPerWrongIngredient = 0.25f;

    [Header("UI Elements")]
    [SerializeField] 
    private TextMeshProUGUI recipeText;

    [Header("Component References")]
    [SerializeField] 
    private ScoreManager scoreManager;

    [SerializeField]
    private CauldronGauge cauldronGauge;

    
    private List<IngredientData> currentRecipe = new List<IngredientData>();

    void Start()
    {
        GenerateNewRecipe();
    }

    public void GenerateNewRecipe()
    {
        currentRecipe.Clear();
        for (int i = 0; i < recipeLength; i++)
        {
            int randomIndex = Random.Range(0, allPossibleIngredients.Length);
            currentRecipe.Add(allPossibleIngredients[randomIndex]);
        }
        UpdateRecipeUI();
        
        if (cauldronGauge != null)
        {
            cauldronGauge.SetHeat(0f);
        }

        // --- NEW ---
        // 3. Restart the spawner with the new settings
        if (ingredientSpawner != null)
        {
            ingredientSpawner.StartSpawning();
        }
    }

    private void UpdateRecipeUI()
    {
        string recipeString = "Rezept:\n"; 
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
                HandleRecipeComplete(); 
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
            else
            {
                Debug.LogError("CauldronGauge is not assigned in the RecipeManager!");
            }
        }
    }

    private void HandleRecipeComplete()
    {
        // --- NEW ---
        // 1. Stop the spawner immediately
        if (ingredientSpawner != null)
        {
            ingredientSpawner.StopSpawning();
        }
        else
        {
            Debug.LogError("IngredientSpawner is not assigned in the RecipeManager!");
        }

        // 2. Increment our level counter
        recipesCompleted++;
        
        float normalizedDifficulty = (float)recipesCompleted / (float)recipesToMaxLevel;
        normalizedDifficulty = Mathf.Clamp01(normalizedDifficulty); 

        Debug.Log("Recipe " + recipesCompleted + " complete! Difficulty is now: " + normalizedDifficulty);

        if (levelArrow != null)
        {
            levelArrow.SetValue(normalizedDifficulty);
        }
        else
        {
            Debug.LogError("LevelArrow is not assigned in the RecipeManager!");
        }

        if (ingredientSpawner != null)
        {
            ingredientSpawner.UpdateDifficulty(normalizedDifficulty);
        }
        else
        {
            Debug.LogError("IngredientSpawner is not assigned in the RecipeManager!");
        }
    }
}