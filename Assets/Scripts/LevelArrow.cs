using UnityEngine;

/// <summary>
/// Controls the rotation of the level arrow based on a normalized value (0 to 1).
/// </summary>
public class LevelArrow : MonoBehaviour
{
    // --- CONFIGURATION ---
    [SerializeField]
    private float minRotationZ = 270f;
    
    [SerializeField]
    private float maxRotationZ = 360f; // 360 is the same as 0

    private Transform arrowTransform;

    void Awake()
    {
        arrowTransform = this.transform;
        // Start the arrow at the minimum rotation
        SetValue(0f); 
    }

    /// <summary>
    /// Sets the arrow's rotation to a specific normalized value.
    /// </summary>
    /// <param name="normalizedValue">The new value, from 0 (min) to 1 (max).</param>
    public void SetValue(float normalizedValue)
    {
        // Clamp the value just in case
        float clampedValue = Mathf.Clamp01(normalizedValue);

        // Calculate the target rotation angle
        float targetAngle = Mathf.Lerp(minRotationZ, maxRotationZ, clampedValue);

        // Apply the rotation
        arrowTransform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}