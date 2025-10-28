using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns random ingredient prefabs at a fixed point and a set interval.
/// Can be started and stopped by other scripts.
/// </summary>
public class IngredientSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] 
    private GameObject[] ingredientPrefabs;

    [SerializeField]
    private float initialSpawnInterval = 2.0f;

    [SerializeField]
    private float fastestSpawnInterval = 0.5f;
    
    [SerializeField] 
    private Vector2 spawnPosition = new Vector2(0f, 4f);

    private float currentSpawnInterval;
    
    // --- NEW ---
    // This flag will control our spawning loop.
    private bool isSpawning = false;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval; 
        StartSpawning(); // Start spawning when the game begins
    }

    public void UpdateDifficulty(float normalizedDifficulty)
    {
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, fastestSpawnInterval, normalizedDifficulty);
        Debug.Log("New Spawn Interval: " + currentSpawnInterval);
    }

    // --- NEW FUNCTION ---
    /// <summary>
    /// Starts the ingredient spawning coroutine.
    /// </summary>
    public void StartSpawning()
    {
        // Don't start if we are already spawning
        if (isSpawning) return; 

        isSpawning = true;
        StartCoroutine(SpawnIngredientRoutine());
    }

    // --- NEW FUNCTION ---
    /// <summary>
    /// Stops the ingredient spawning coroutine.
    /// </summary>
    public void StopSpawning()
    {
        isSpawning = false;
        // The coroutine will stop by itself when it checks the 'isSpawning' flag
    }


    private IEnumerator SpawnIngredientRoutine()
    {
        // --- UPDATED ---
        // This loop will now stop when 'isSpawning' becomes false
        while (isSpawning)
        {
            if (ingredientPrefabs.Length == 0)
            {
                Debug.LogError("No ingredient prefabs assigned to the spawner!");
                yield break;
            }

            int randomIndex = Random.Range(0, ingredientPrefabs.Length);
            GameObject randomIngredient = ingredientPrefabs[randomIndex];
            Instantiate(randomIngredient, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }
}