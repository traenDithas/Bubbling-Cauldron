using UnityEngine;

/// <summary>
/// UPDATED: Now detects ingredients and passes them to the
/// RecipeManager for checking.
/// </summary>
public class CauldronLogic : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the RecipeManager, which is the brain of the game.")]
    [SerializeField] 
    private RecipeManager recipeManager;

    /// <summary>
    /// Called when another Collider2D enters this object's trigger.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Get the IngredientData from the object.
        IngredientData ingredient = other.GetComponent<IngredientData>();

        // 2. Check if it was an ingredient.
        if (ingredient != null)
        {
            // 3. Pass the ingredient to the RecipeManager to handle.
            if (recipeManager != null)
            {
                recipeManager.OnIngredientCaught(ingredient);
            }
            else
            {
                Debug.LogError("RecipeManager is not assigned on the CauldronLogic script!");
            }

            // 4. Destroy the ingredient GameObject (whether it was right or wrong).
            Destroy(other.gameObject);
        }
    }
}