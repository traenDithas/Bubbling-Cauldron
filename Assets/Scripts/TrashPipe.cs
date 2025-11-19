using UnityEngine;

/// <summary>
/// Attached to the trash pipe. 
/// Only resets streak if the player missed a REQUIRED ingredient.
/// Trash/Special items can be safely dropped here.
/// </summary>
public class TrashPipe : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the ScoreManager to reset streak.")]
    [SerializeField] private ScoreManager scoreManager;

    [Tooltip("Reference to the RecipeManager to check if item was needed.")]
    [SerializeField] private RecipeManager recipeManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IngredientData ingredient = other.GetComponent<IngredientData>();

        if (ingredient != null)
        {
            // Check if this ingredient was actually in the recipe
            bool wasNeeded = false;

            if (recipeManager != null)
            {
                wasNeeded = recipeManager.IsIngredientInCurrentRecipe(ingredient.ingredientID);
            }
            else
            {
                // Failsafe: try to find the manager if we forgot to drag it in
                var manager = FindFirstObjectByType<RecipeManager>();
                if (manager != null) wasNeeded = manager.IsIngredientInCurrentRecipe(ingredient.ingredientID);
            }

            if (wasNeeded)
            {
                // --- PUNISH PLAYER ---
                // They missed an ingredient that they were supposed to catch!
                Debug.Log("Trashed REQUIRED item: " + ingredient.ingredientID + " -> Streak Reset!");
                
                if (scoreManager != null)
                {
                    scoreManager.ResetStreak();
                }
                else
                {
                    FindFirstObjectByType<ScoreManager>()?.ResetStreak();
                }
            }
            else
            {
                // --- NO PUNISHMENT ---
                // This was a wrong ingredient or a special item.
                // It is GOOD that the player let it fall into the trash.
                Debug.Log("Trashed unwanted item: " + ingredient.ingredientID + " -> Safe.");
            }

            // Always destroy the object so it doesn't pile up
            Destroy(other.gameObject);
        }
    }
}