using UnityEngine;

// We removed [RequireComponent(typeof(Rigidbody2D))] because we are 3D now
public class IngredientData : MonoBehaviour
{
    [Header("Ingredient Info")]
    public string ingredientID;
    public int scoreValue;

    [Tooltip("Drag the Texture/Image of the ingredient here.")]
    public Texture iconTexture; 

    // Changed to 3D Rigidbody
    private Rigidbody rb;

    void Awake()
    {
        // Get the 3D component
        rb = GetComponent<Rigidbody>();
    }

    public void SetFallSpeed(float gravityScale)
    {
        if (rb != null)
        {
            // We can add custom gravity logic here later if needed
        }
    }
}