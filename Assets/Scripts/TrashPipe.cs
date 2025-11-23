using UnityEngine;

public class TrashPipe : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private RecipeManager recipeManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IngredientData ingredient = other.GetComponent<IngredientData>();

        if (ingredient != null)
        {
            // --- STAT TRACKING ---
            if (scoreManager != null)
            {
                scoreManager.RecordTrashedIngredient();
            }
            // --------------------

            bool wasNeeded = false;
            if (recipeManager != null) wasNeeded = recipeManager.IsIngredientInCurrentRecipe(ingredient.ingredientID);
            else 
            {
                var manager = FindFirstObjectByType<RecipeManager>();
                if (manager != null) wasNeeded = manager.IsIngredientInCurrentRecipe(ingredient.ingredientID);
            }

            if (wasNeeded)
            {
                if (scoreManager != null) scoreManager.ResetStreak();
                else FindFirstObjectByType<ScoreManager>()?.ResetStreak();
            }

            Destroy(other.gameObject);
        }
    }
}