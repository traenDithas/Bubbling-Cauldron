using UnityEngine;

/// <summary>
/// Controls the Sorter Arm's rotation to be either in a 
/// resting or active (flipper) position.
/// </summary>
public class SorterController : MonoBehaviour
{
    [Header("Arm Setup")]
    [Tooltip("The Transform of the Sorter_Arm object that will rotate.")]
    [SerializeField] 
    private Transform sorterArm;

    [Tooltip("The Z-axis rotation of the arm in its resting state (e.g., 0).")]
    [SerializeField] 
    private float restingRotationZ = 0f;

    [Tooltip("The Z-axis rotation of the arm when it is active (e.g., 45).")]
    [SerializeField] 
    private float activeRotationZ = 45f;

    /// <summary>
    /// Sets the arm's rotation to the active or resting state.
    /// This function will be called by the UI button's EventTrigger.
    /// </summary>
    /// <param name="isActive">True to move to active, False to move to rest.</param>
    public void SetArmActive(bool isActive)
    {
        // 1. Determine the target angle.
        // This is a "ternary operator," a short way to write an if/else statement.
        float targetAngle = isActive ? activeRotationZ : restingRotationZ;

        // 2. Apply the rotation.
        // We use Quaternion.Euler to convert our Z-angle into a 3D rotation.
        if (sorterArm != null)
        {
            sorterArm.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
        else
        {
            Debug.LogError("Sorter Arm Transform is not assigned!");
        }
    }
}