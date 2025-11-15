using UnityEngine;
using System.Collections;
using System.Collections.Generic; // We need this for Lists
using System.Linq; // We need this for the .Sum() function

// --- NEW HELPER CLASS 1 ---
/// <summary>
/// A small class to hold one ingredient and its spawn chance.
/// We don't need to attach this to any GameObject.
/// </summary>
[System.Serializable]
public class IngredientSpawnData
{
    [Tooltip("The ingredient prefab to spawn (e.g., Egg, Fish).")]
    public GameObject prefab;

    [Tooltip("The relative chance for this to spawn. If Egg=75 and Fish=25, Egg will spawn 75% of the time.")]
    public float spawnWeight = 1.0f;
}

// --- NEW HELPER CLASS 2 ---
/// <summary>
/// A class to hold the list of ingredients for a single level.
/// </summary>
[System.Serializable]
public class LevelIngredientList
{
    [Tooltip("Just for organizing in the Inspector (e.g., 'Level 1 - Easy').")]
    public string levelName;

    [Tooltip("The list of all ingredients that can spawn on this level.")]
    public List<IngredientSpawnData> ingredients;
}

// --- UPDATED MAIN SCRIPT ---
public class IngredientSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [Tooltip("The initial time between spawns when the game starts.")]
    [SerializeField] private float initialSpawnInterval = 2.0f;
    
    [Tooltip("The shortest possible time between spawns at max difficulty.")]
    [SerializeField] private float fastestSpawnInterval = 0.5f;

    [Header("Difficulty Settings")]
    [Tooltip("The falling speed (gravity) at the start of the game.")]
    [SerializeField] private float initialGravityScale = 1.0f;

    [Tooltip("The fastest falling speed (gravity) at max difficulty.")]
    [SerializeField] private float maxGravityScale = 2.5f;

    [Header("Assembly Movement")]
    [Tooltip("The lowest Y-position this Pipe object will move to.")]
    [SerializeField] private float spawnerLowestY = -1.5f;
    [SerializeField] private Vector3 spawnPositionOffset = Vector3.zero;
    
    // --- THIS REPLACES THE OLD PREFAB LIST ---
    [Header("Level-Based Spawning Data")]
    [Tooltip("Add one element for each level in your game (e.g., Size 9).")]
    [SerializeField] private List<LevelIngredientList> levelData;

    // --- Private Variables ---
    private float currentSpawnInterval;
    private bool isSpawning = false;
    private float spawnerInitialY;
    private float currentNormalizedDifficulty = 0f;
    private int currentLevelIndex = 0; // Tracks which level data to use

    void Start()
    {
        spawnerInitialY = transform.position.y;
        currentSpawnInterval = initialSpawnInterval;
        currentNormalizedDifficulty = 0f;
        currentLevelIndex = 0;
    }

    public void UpdateDifficulty(float normalizedDifficulty)
    {
        currentNormalizedDifficulty = normalizedDifficulty;
        
        // 1. Update Interval
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, fastestSpawnInterval, normalizedDifficulty);
        
        // 2. Update Movement
        float newSpawny = Mathf.Lerp(spawnerInitialY, spawnerLowestY, normalizedDifficulty);
        transform.position = new Vector3(transform.position.x, newSpawny, transform.position.z);
        
        // 3. --- NEW --- Update the current level index
        if (levelData.Count > 0)
        {
            int newLevelIndex = Mathf.FloorToInt(normalizedDifficulty * levelData.Count);
            currentLevelIndex = Mathf.Clamp(newLevelIndex, 0, levelData.Count - 1);
        }
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

    /// <summary>
    /// Gets a random prefab based on the weighted chances for the current level.
    /// </summary>
    private GameObject GetRandomPrefabFromList()
    {
        if (levelData.Count == 0) return null; // No level data assigned

        List<IngredientSpawnData> currentIngredients = levelData[currentLevelIndex].ingredients;
        if (currentIngredients.Count == 0) return null; // This level has no ingredients

        // Calculate the total weight of all ingredients in this level's list
        float totalWeight = currentIngredients.Sum(item => item.spawnWeight);

        if (totalWeight <= 0)
        {
            Debug.LogWarning("Total spawn weight for level " + currentLevelIndex + " is 0. Returning first ingredient.");
            return currentIngredients[0].prefab;
        }

        // Get a random value between 0 and the total weight
        float randomValue = Random.Range(0, totalWeight);

        // Loop through the list until we find the ingredient
        foreach (var item in currentIngredients)
        {
            if (randomValue <= item.spawnWeight)
            {
                return item.prefab;
            }
            randomValue -= item.spawnWeight; // Subtract this item's weight and check the next
        }

        // Fallback (shouldn't be hit, but good practice)
        return currentIngredients[0].prefab;
    }

    private IEnumerator SpawnIngredientRoutine()
    {
        yield return new WaitForSeconds(currentSpawnInterval);
        
        while (isSpawning)
        {
            // --- UPDATED SPAWNING LOGIC ---
            
            // 1. Get a random prefab using our new weighted logic
            GameObject prefabToSpawn = GetRandomPrefabFromList();

            if (prefabToSpawn == null)
            {
                Debug.LogError("No ingredients assigned to spawn for current level: " + currentLevelIndex);
                yield break; // Stop spawning
            }

            // 2. Create the ingredient
            Vector3 spawnPoint = transform.position + spawnPositionOffset;
            GameObject newIngredient = Instantiate(prefabToSpawn, spawnPoint, Quaternion.identity);

            // 3. Calculate and set its fall speed
            float newGravity = Mathf.Lerp(initialGravityScale, maxGravityScale, currentNormalizedDifficulty);
            IngredientData data = newIngredient.GetComponent<IngredientData>();
            if (data != null)
            {
                data.SetFallSpeed(newGravity);
            }
            // --- END UPDATED ---

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }
}