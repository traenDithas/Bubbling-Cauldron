using UnityEngine;

/// <summary>
/// Controls the rotation of the cauldron's needle based on a normalized value (0 to 1).
/// </summary>
public class CauldronGauge : MonoBehaviour
{
    // --- CONFIGURATION ---
    // These are the start and end rotation values for the needle in degrees.
    // We make them visible in the Inspector so we can tweak them easily.
    [SerializeField]
    private float minRotationZ = 65f;
    
    [SerializeField]
    private float maxRotationZ = -92f;

    // --- REGULATOR ---
    // This is our public "regulator". The [Range(0, 1)] attribute turns it into
    // a handy slider in the Inspector, perfect for testing!
    [Tooltip("The current value of the gauge, from 0 (min) to 1 (max).")]
    [Range(0, 1)]
    public float gaugeValue;

    // --- INTERNAL VARIABLES ---
    // A reference to the needle's Transform component to make it rotate.
    private Transform needleTransform;

    /// <summary>
    /// This method is called once when the script instance is being loaded.
    /// It's the best place to set up our references.
    /// </summary>
    void Awake()
    {
        // Get the Transform component of the GameObject this script is attached to.
        needleTransform = this.transform;
    }

    /// <summary>
    /// This method is called every frame, if the MonoBehaviour is enabled.
    /// We use it to continuously update the needle's rotation.
    /// </summary>
    void Update()
    {
        // Calculate the target rotation angle. Mathf.Lerp() is perfect for this.
        // It "linearly interpolates" between the min and max rotation based on our gaugeValue.
        float targetAngle = Mathf.Lerp(minRotationZ, maxRotationZ, gaugeValue);

        // Apply the rotation to the needle.
        // We use Quaternion.Euler() to convert our Z-angle into a rotation Unity understands.
        needleTransform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}