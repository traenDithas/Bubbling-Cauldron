using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class IngredientData : MonoBehaviour
{
    [Header("Ingredient Info")]
    public string ingredientID;
    public int scoreValue;

    // --- CHANGE: We now want the Texture (Image), not the Material ---
    [Tooltip("Drag the Texture/Image of the ingredient here.")]
    public Texture iconTexture; 
    // -----------------------------------------------------------------

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetFallSpeed(float gravityScale)
    {
        if (rb != null)
        {
            rb.gravityScale = gravityScale;
        }
    }
}