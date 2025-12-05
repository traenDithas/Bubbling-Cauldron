using UnityEngine;

public class RecipeIcon : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Renderer iconRenderer; 

    [Tooltip("The EXACT name of the Fill property in your Shader Graph (usually _Fill or _FillAmount).")]
    [SerializeField] private string fillPropertyName = "_Fill"; 
    
    [Tooltip("The EXACT name of the Boolean property for unknown state.")]
    [SerializeField] private string unknownPropertyName = "_Unknown"; 

    private Material materialInstance;
    private float currentFill = 0f;
    private int fillGoal = 1; 

    public string ingredientID { get; private set; }

    void Awake()
    {
        if (iconRenderer == null) iconRenderer = GetComponent<Renderer>();

        if (iconRenderer != null)
        {
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
            // Use the variable name, not a hardcoded string
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