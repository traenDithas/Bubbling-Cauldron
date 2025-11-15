using UnityEngine;

/// <summary>
/// This component holds the essential data for an ingredient.
/// It also now controls its own fall speed.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))] // Good practice: ensures we have a Rigidbody
public class IngredientData : MonoBehaviour
{
    [Header("Ingredient Info")]
    [Tooltip("A unique ID for this ingredient, e.g., 'Auge' or 'Pilz'.")]
    public string ingredientID;
    
    [Tooltip("The number of points this ingredient is worth.")]
    public int scoreValue;

    // --- NEW ---
    // A private reference to this ingredient's Rigidbody
    private Rigidbody2D rb;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// A new public function that allows other scripts (like the spawner)
    /// to set this ingredient's fall speed.
    /// </summary>
    /// <param name="gravityScale">The new gravity scale. 1 is default, 2 is twice as fast.</param>
    public void SetFallSpeed(float gravityScale)
    {
        if (rb != null)
        {
            rb.gravityScale = gravityScale;
        }
    }
    // --- END NEW ---
}