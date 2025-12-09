using UnityEngine;

public class RecipeIcon : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Renderer iconRenderer; 

    [Tooltip("The EXACT name of the Fill property in your Shader.")]
    [SerializeField] private string fillPropertyName = "_Fill"; 
    
    [Tooltip("The EXACT name of the Unknown property.")]
    [SerializeField] private string unknownPropertyName = "_Unknown"; 

    // --- NEW: The name of the Texture property ---
    [Tooltip("The name of the Texture property in the shader (usually _MainTex or _BaseMap).")]
    [SerializeField] private string texturePropertyName = "_MainTex";
    // ---------------------------------------------

    private Material materialInstance;
    private float currentFill = 0f;
    private int fillGoal = 1; 

    public string ingredientID { get; private set; }

    void Awake()
    {
        if (iconRenderer == null) iconRenderer = GetComponent<Renderer>();

        if (iconRenderer != null)
        {
            // This creates a UNIQUE copy of the material for this icon
            materialInstance = iconRenderer.material;
        }
    }

    public void Initialize(string id, int targetAmount)
    {
        ingredientID = id;
        fillGoal = targetAmount;
        SetFill(0f); 
        SetUnknownState(false); 
    }

    // --- NEW: Update the Texture Only ---
    public void SetVisual(Texture newTexture)
    {
        if (materialInstance != null && newTexture != null)
        {
            // Keep the shader, just change the picture!
            materialInstance.SetTexture(texturePropertyName, newTexture);
        }
    }
    // ------------------------------------

    public void UpdateFill(int currentCount)
    {
        currentFill = Mathf.Clamp01((float)currentCount / fillGoal);
        SetFill(currentFill);
        
        if (currentCount > 0) SetUnknownState(false);
    }

    private void SetFill(float amount)
    {
        if (materialInstance != null)
        {
            materialInstance.SetFloat(fillPropertyName, amount);
        }
    }

    public void SetUnknownState(bool isUnknown)
    {
        if (materialInstance != null)
        {
            materialInstance.SetInt(unknownPropertyName, isUnknown ? 1 : 0);
        }
    }
}