using UnityEngine;

/// <summary>
/// Attached to the trash pipe. Destroys any GameObject with an
/// IngredientData component that enters its trigger.
/// </summary>
public class TrashPipe : MonoBehaviour
{
    /// <summary>
    /// This method is called by Unity's physics engine when another
    /// Collider2D enters this object's trigger.
    /// </summary>
    /// <param name="other">The collider of the object that entered.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Try to get the IngredientData component from the object that fell in.
        IngredientData ingredient = other.GetComponent<IngredientData>();

        // 2. Check if the object was an ingredient.
        if (ingredient != null)
        {
            // Yes, it's an ingredient. Destroy it.
            Debug.Log("Trashed: " + ingredient.ingredientID); // Good for testing!
            Destroy(other.gameObject);
        }
        
        // If it's not an ingredient (something else), we'll just ignore it.
    }
}