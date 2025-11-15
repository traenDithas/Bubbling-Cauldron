using UnityEngine;
using System.Collections;

public class IngredientSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private GameObject[] ingredientPrefabs;
    [SerializeField] private float initialSpawnInterval = 2.0f;
    [SerializeField] private float fastestSpawnInterval = 0.5f;

    // --- NEW SECTION ---
    [Header("Difficulty Settings")]
    [Tooltip("The falling speed (gravity) at the start of the game.")]
    [SerializeField] private float initialGravityScale = 1.0f;

    [Tooltip("The fastest falling speed (gravity) at max difficulty.")]
    [SerializeField] private float maxGravityScale = 2.5f;
    // --- END NEW ---

    [Header("Assembly Movement")]
    [SerializeField] private float spawnerLowestY = -1.5f;
    [SerializeField] private Vector3 spawnPositionOffset = Vector3.zero;

    // Private Variables
    private float currentSpawnInterval;
    private bool isSpawning = false;
    private float spawnerInitialY;

    // --- NEW ---
    // We need to store the current difficulty to apply it to new ingredients
    private float currentNormalizedDifficulty = 0f;
    // --- END NEW ---

    void Start()
    {
        spawnerInitialY = transform.position.y;
        currentSpawnInterval = initialSpawnInterval;
        // We set the difficulty to 0 (easy) at the very start
        currentNormalizedDifficulty = 0f;
    }

    public void UpdateDifficulty(float normalizedDifficulty)
    {
        // --- NEW ---
        // Store the current difficulty level
        currentNormalizedDifficulty = normalizedDifficulty;
        // --- END NEW ---
        
        // 1. Interval Update
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, fastestSpawnInterval, normalizedDifficulty);
        
        // 2. Movement Update
        float newSpawny = Mathf.Lerp(spawnerInitialY, spawnerLowestY, normalizedDifficulty);
        transform.position = new Vector3(transform.position.x, newSpawny, transform.position.z);
    }

    public void StartSpawning()
    {
        if (isSpawning) return;
        isSpawning = true;
        StartCoroutine(SpawnIngredientRoutine());
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    private IEnumerator SpawnIngredientRoutine()
    {
        yield return new WaitForSeconds(currentSpawnInterval);
        
        while (isSpawning)
        {
            if (ingredientPrefabs.Length == 0)
            {
                Debug.LogError("No ingredient prefabs assigned to the spawner!");
                yield break;
            }

            int randomIndex = Random.Range(0, ingredientPrefabs.Length);
            
            // --- UPDATED SPAWNING LOGIC ---

            // 1. Create the ingredient and save a reference to it
            Vector3 spawnPoint = transform.position + spawnPositionOffset;
            GameObject newIngredient = Instantiate(ingredientPrefabs[randomIndex], spawnPoint, Quaternion.identity);

            // 2. Calculate the correct gravity for the current level
            float newGravity = Mathf.Lerp(initialGravityScale, maxGravityScale, currentNormalizedDifficulty);

            // 3. Get the IngredientData script from the new clone
            IngredientData data = newIngredient.GetComponent<IngredientData>();

            // 4. Tell the ingredient to set its fall speed
            if (data != null)
            {
                data.SetFallSpeed(newGravity);
            }
            // --- END UPDATED ---

            // Wait for the correct, updated interval
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }
}