using UnityEngine;

/// <summary>
/// Attached to the trash pipe. 
/// If an ingredient falls here, it resets the player's streak.
/// </summary>
public class TrashPipe : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the ScoreManager to reset streak.")]
    [SerializeField] private ScoreManager scoreManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IngredientData ingredient = other.GetComponent<IngredientData>();

        if (ingredient != null)
        {
            Debug.Log("Trashed: " + ingredient.ingredientID);
            
            // --- STREAK BREAKER ---
            // The player missed the ingredient, so the streak resets.
            if (scoreManager != null)
            {
                scoreManager.ResetStreak();
            }
            else
            {
                // Try to find it if not assigned (failsafe)
                // FIX: We use FindFirstObjectByType instead of FindObjectOfType
                FindFirstObjectByType<ScoreManager>()?.ResetStreak();
            }

            Destroy(other.gameObject);
        }
    }
}