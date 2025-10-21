using System.Collections;
using System.Collections.Generic; // We need this for Lists!
using UnityEngine;
using TMPro; // We need this for the TextMeshPro UI

/// <summary>
/// The "brain" of the game. Generates new recipes, displays them on the UI,
/// and checks if the player caught the correct ingredient.
/// </summary>
public class RecipeManager : MonoBehaviour
{
    [Header("Recipe Settings")]
    [Tooltip("A list of ALL possible ingredient prefabs the game can choose from.")]
    [SerializeField] 
    private IngredientData[] allPossibleIngredients;

    [Tooltip("The number of ingredients in one recipe.")]
    [SerializeField] 
    private int recipeLength = 4;

    [Header("UI Elements")]
    [Tooltip("The TextMeshPro UI element that displays the recipe list.")]
    [SerializeField] 
    private TextMeshProUGUI recipeText;

    [Header("Component References")]
    [Tooltip("Reference to the ScoreManager to add points.")]
    [SerializeField] 
    private ScoreManager scoreManager;

    // This list stores the currently active recipe.
    private List<IngredientData> currentRecipe = new List<IngredientData>();

    /// <summary>
    /// Called when the game starts.
    /// </summary>
    void Start()
    {
        GenerateNewRecipe();
    }

    /// <summary>
    /// Generates a new random recipe and updates the UI.
    /// </summary>
    public void GenerateNewRecipe()
    {
        // Clear the old recipe first.
        currentRecipe.Clear();

        // Create the new recipe by picking random ingredients.
        for (int i = 0; i < recipeLength; i++)
        {
            int randomIndex = Random.Range(0, allPossibleIngredients.Length);
            currentRecipe.Add(allPossibleIngredients[randomIndex]);
        }

        // Update the text on the scroll.
        UpdateRecipeUI();
    }

    /// <summary>
    /// Updates the recipe scroll text to match the currentRecipe list.
    /// </summary>
    private void UpdateRecipeUI()
    {
        // Start with the title.
        string recipeString = "Rezept:\n"; // "Rezept" is German for "Recipe"

        if (currentRecipe.Count == 0)
        {
            recipeString += "Fertig!"; // "Complete!"
        }
        else
        {
            // Add each ingredient's ID to the text.
            foreach (IngredientData ingredient in currentRecipe)
            {
                recipeString += "- " + ingredient.ingredientID + "\n";
            }
        }
        
        recipeText.text = recipeString;
    }

    /// <summary>
    /// This is called by the CauldronLogic when an ingredient is caught.
    /// </summary>
    /// <param name="caughtIngredient">The IngredientData of the item that fell in.</param>
    public void OnIngredientCaught(IngredientData caughtIngredient)
    {
        bool wasInRecipe = false;

        // Loop backwards (safer when removing from a list)
        for (int i = currentRecipe.Count - 1; i >= 0; i--)
        {
            // Check if the caught ingredient's ID matches one in the recipe.
            if (currentRecipe[i].ingredientID == caughtIngredient.ingredientID)
            {
                // --- CORRECT INGREDIENT! ---
                wasInRecipe = true;

                // 1. Add score.
                scoreManager.AddScore(caughtIngredient.scoreValue);

                // 2. Remove it from the recipe list.
                currentRecipe.RemoveAt(i);

                // 3. Stop searching.
                break;
            }
        }

        if (wasInRecipe)
        {
            // --- GOOD JOB! ---
            Debug.Log("Correct ingredient: " + caughtIngredient.ingredientID);
            
            // Refresh the UI to show the item being checked off.
            UpdateRecipeUI();

            // Check if the recipe is now complete.
            if (currentRecipe.Count == 0)
            {
                Debug.Log("Recipe Complete! Generating new one.");
                // Wait 1.5 seconds, then make a new recipe.
                Invoke("GenerateNewRecipe", 1.5f);
            }
        }
        else
        {
            // --- WRONG INGREDIENT! ---
            Debug.Log("Wrong ingredient: " + caughtIngredient.ingredientID);
            // We could subtract points here if we wanted!
            // scoreManager.AddScore(-5);
        }
    }
}