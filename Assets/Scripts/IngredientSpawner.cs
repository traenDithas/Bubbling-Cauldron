using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns random ingredient prefabs at a fixed point and a set interval.
/// </summary>
public class IngredientSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("A list of all ingredient prefabs that can be spawned.")]
    [SerializeField] 
    private GameObject[] ingredientPrefabs;

    // --- UPDATED SPAWN INTERVAL ---
    [Tooltip("The spawn speed at the start (e.g., 2.0 seconds).")]
    [SerializeField]
    private float initialSpawnInterval = 2.0f;

    [Tooltip("The fastest spawn speed at max level (e.g., 0.5 seconds).")]
    [SerializeField]
    private float fastestSpawnInterval = 0.5f;
    
    [Tooltip("The exact position where ingredients will be created.")]
    [SerializeField] 
    private Vector2 spawnPosition = new Vector2(0f, 4f);

    // This will hold the spawner's current speed
    private float currentSpawnInterval; 

    void Start()
    {
        // Set the speed to the starting speed
        currentSpawnInterval = initialSpawnInterval; 
        StartCoroutine(SpawnIngredientRoutine());
    }

    // --- NEW PUBLIC FUNCTION ---
    /// <summary>
    /// Updates the spawner's speed based on the game's difficulty.
    /// </summary>
    /// <param name="normalizedDifficulty">A value from 0 (easy) to 1 (hard).</param>
    public void UpdateDifficulty(float normalizedDifficulty)
    {
        // Lerp between the initial and fastest speed
        // As difficulty (0-1) goes UP, interval (2.0-0.5) goes DOWN
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, fastestSpawnInterval, normalizedDifficulty);
        Debug.Log("New Spawn Interval: " + currentSpawnInterval); // For testing
    }

    private IEnumerator SpawnIngredientRoutine()
    {
        while (true)
        {
            if (ingredientPrefabs.Length == 0)
            {
                Debug.LogError("No ingredient prefabs assigned to the spawner!");
                yield break;
            }

            int randomIndex = Random.Range(0, ingredientPrefabs.Length);
            GameObject randomIngredient = ingredientPrefabs[randomIndex];
            Instantiate(randomIngredient, spawnPosition, Quaternion.identity);

            // --- UPDATED WAIT TIME ---
            // Wait for the *current* interval, which can change
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }
}