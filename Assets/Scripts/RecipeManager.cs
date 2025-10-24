using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeManager : MonoBehaviour
{
    [Header("Recipe Settings")]
    [SerializeField] 
    private IngredientData[] allPossibleIngredients;

    [SerializeField] 
    private int recipeLength = 4;

    // --- NEW VARIABLE ---
    [Tooltip("How much heat (0-1) to add for each wrong ingredient.")]
    [SerializeField]
    private float heatPerWrongIngredient = 0.25f; // e.g., 4 wrong items = full heat

    [Header("UI Elements")]
    [SerializeField] 
    private TextMeshProUGUI recipeText;

    [Header("Component References")]
    [SerializeField] 
    private ScoreManager scoreManager;

    // --- NEW REFERENCE ---
    [Tooltip("Reference to the CauldronGauge script (on the Needle).")]
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
        
        // --- NEW ---
        // Let's also reset the heat when we make a new recipe.
        if (cauldronGauge != null)
        {
            cauldronGauge.SetHeat(0f);
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
                Invoke("GenerateNewRecipe", 1.5f);
            }
        }
        else
        {
            // --- WRONG INGREDIENT! ---
            Debug.Log("Wrong ingredient: " + caughtIngredient.ingredientID);
            
            // --- NEW CODE HERE ---
            // Tell the gauge to add heat!
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
}