using UnityEngine;

/// <summary>
/// Controls the rotation of the cauldron's needle based on an
/// internal heat value (0 to 1).
/// </summary>
public class CauldronGauge : MonoBehaviour
{
    // --- CONFIGURATION ---
    [SerializeField]
    private float minRotationZ = 65f;
    
    [SerializeField]
    private float maxRotationZ = -92f;

    // --- STATE ---
    [Tooltip("The current heat value, from 0 (min) to 1 (max).")]
    // This is now private and controlled by our functions!
    private float currentHeatValue = 0f;

    // --- INTERNAL VARIABLES ---
    private Transform needleTransform;

    void Awake()
    {
        needleTransform = this.transform;
        // Set the needle to the starting position.
        UpdateNeedleRotation();
    }

    /// <summary>
    /// We've removed the Update() method for now, as the needle
    /// will only update when we call AddHeat or SetHeat.
    /// (We can add it back later if we want it to cool down over time).
    /// </small>

    /// <summary>
    /// Updates the needle's visual rotation based on the currentHeatValue.
    /// </summary>
    private void UpdateNeedleRotation()
    {
        // Calculate the target rotation angle.
        float targetAngle = Mathf.Lerp(minRotationZ, maxRotationZ, currentHeatValue);

        // Apply the rotation.
        needleTransform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    // --- PUBLIC FUNCTIONS ---

    /// <summary>
    /// Adds a specified amount to the heat and updates the needle.
    /// </summary>
    /// <param name="amountToAdd">The 0-1 amount of heat to add.</param>
    public void AddHeat(float amountToAdd)
    {
        // Add the heat, but clamp it so it never goes above 1.0.
        currentHeatValue = Mathf.Clamp01(currentHeatValue + amountToAdd);
        
        Debug.Log("Heat is now: " + currentHeatValue); // For testing
        
        // Update the needle's position.
        UpdateNeedleRotation();
    }

    /// <summary>
    /// Sets the heat to a specific value (e.g., 0 to reset it).
    /// </summary>
    /// <param name="newValue">The new 0-1 heat value.</param>
    public void SetHeat(float newValue)
    {
        currentHeatValue = Mathf.Clamp01(newValue);
        
        // Update the needle's position.
        UpdateNeedleRotation();
    }
}