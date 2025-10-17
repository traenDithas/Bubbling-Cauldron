using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns random ingredient prefabs at a fixed point and a set interval.
/// </summary>
public class IngredientSpawner : MonoBehaviour
{
    // --- CONFIGURATION ---
    [Header("Spawning Settings")]
    [Tooltip("A list of all ingredient prefabs that can be spawned.")]
    [SerializeField] 
    private GameObject[] ingredientPrefabs;

    [Tooltip("The time in seconds between each spawn.")]
    [SerializeField] 
    private float spawnInterval = 2.0f;

    // The fixed position where ingredients will be created.
    private Vector2 spawnPosition = new Vector2(0f, 4f);

    /// <summary>
    /// This method is called when the game starts.
    /// We use it to kick off the spawning process.
    /// </summary>
    void Start()
    {
        // Start the SpawnIngredient coroutine.
        StartCoroutine(SpawnIngredientRoutine());
    }

    /// <summary>
    /// A Coroutine that runs in the background to spawn ingredients.
    /// This allows us to wait for a certain amount of time without freezing the game.
    /// </summary>
    private IEnumerator SpawnIngredientRoutine()
    {
        // This is an infinite loop that will run for the entire game.
        while (true)
        {
            // --- SPAWNING LOGIC ---

            // 1. Check if we have any prefabs to spawn. If not, log an error.
            if (ingredientPrefabs.Length == 0)
            {
                Debug.LogError("No ingredient prefabs assigned to the spawner!");
                yield break; // Stop the coroutine.
            }

            // 2. Pick a random index from our array.
            int randomIndex = Random.Range(0, ingredientPrefabs.Length);

            // 3. Get the random ingredient prefab from the array.
            GameObject randomIngredient = ingredientPrefabs[randomIndex];

            // 4. Create a new instance of the ingredient at the spawn position.
            // Quaternion.identity means "no rotation".
            Instantiate(randomIngredient, spawnPosition, Quaternion.identity);

            // --- WAIT ---
            // Wait for 'spawnInterval' seconds before the loop runs again.
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}