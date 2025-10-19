using UnityEngine;

/// <summary>
/// Attached to the Cauldron. Detects ingredients entering its trigger,
/// tells the ScoreManager to add points, and then destroys the ingredient.
/// </summary>
public class CauldronLogic : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the ScoreManager to add points.")]
    [SerializeField] 
    private ScoreManager scoreManager;

    /// <summary>
    /// This method is called by Unity's physics engine when another
    /// Collider2D enters this object's trigger.
    /// </summary>
    /// <param name="other">The collider of the object that entered.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Try to get the IngredientData component from the object that fell in.
        IngredientData ingredient = other.GetComponent<IngredientData>();

        // 2. Check if the object that fell in was actually an ingredient.
        if (ingredient != null)
        {
            // --- IT IS AN INGREDIENT! ---

            // Log to the console for testing (you can remove this later)
            Debug.Log("Caught: " + ingredient.ingredientID + ", Worth: " + ingredient.scoreValue);

            // 3. Tell the ScoreManager to add this ingredient's score value.
            if (scoreManager != null)
            {
                scoreManager.AddScore(ingredient.scoreValue);
            }
            else
            {
                Debug.LogError("ScoreManager is not assigned on the CauldronLogic script!");
            }

            // 4. Destroy the ingredient GameObject.
            Destroy(other.gameObject);
        }
        else
        {
            // --- IT IS NOT AN INGREDIENT ---
            // Something else fell in (like the player, maybe?). We'll ignore it.
            // We could also destroy it if we don't want it piling up.
            // Destroy(other.gameObject); 
        }
    }
}