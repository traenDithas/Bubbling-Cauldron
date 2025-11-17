using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// --- Helper classes ---
[System.Serializable]
public class IngredientSpawnData
{
    public GameObject prefab;
    public float spawnWeight = 1.0f;
}

[System.Serializable]
public class LevelIngredientList
{
    public string levelName;
    public List<IngredientSpawnData> ingredients;
}

// --- Main Spawner Script ---
public class IngredientSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private float initialSpawnInterval = 2.0f;
    [SerializeField] private float fastestSpawnInterval = 0.5f;

    [Header("Difficulty Settings")]
    [SerializeField] private float initialGravityScale = 1.0f;
    [SerializeField] private float maxGravityScale = 2.5f;

    [Header("Assembly Movement")]
    [SerializeField] private float spawnerLowestY = -1.5f;
    [SerializeField] private Vector3 spawnPositionOffset = Vector3.zero;
    
    [Header("Level-Based Spawning Data")]
    [SerializeField] private List<LevelIngredientList> levelData;

    // --- Private Variables ---
    private float currentSpawnInterval;
    private bool isSpawning = false;
    private float spawnerInitialY; // This is the variable we need to set early
    private float currentNormalizedDifficulty = 0f;
    private int currentLevelIndex = 0;

    // --- THIS IS THE FIX ---
    // Awake() runs before any Start() methods, so we guarantee
    // 'spawnerInitialY' is set before the RecipeManager tries to call
    // UpdateDifficulty().
    void Awake()
    {
        spawnerInitialY = transform.position.y;
        currentSpawnInterval = initialSpawnInterval;
        currentNormalizedDifficulty = 0f;
        currentLevelIndex = 0;
    }
    // --- END OF FIX ---

    public void UpdateDifficulty(float normalizedDifficulty, int completedLevelCount)
    {
        currentNormalizedDifficulty = normalizedDifficulty;
        
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, fastestSpawnInterval, normalizedDifficulty);
        
        // This Lerp will now work correctly because spawnerInitialY is set
        float newSpawny = Mathf.Lerp(spawnerInitialY, spawnerLowestY, normalizedDifficulty);
        transform.position = new Vector3(transform.position.x, newSpawny, transform.position.z);
        
        currentLevelIndex = Mathf.Clamp(completedLevelCount, 0, levelData.Count - 1);
        
        Debug.Log("Recipe complete! Now using level list index: " + currentLevelIndex);
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

    private GameObject GetRandomPrefabFromList()
    {
        if (levelData.Count == 0 || levelData.Count <= currentLevelIndex) return null;

        List<IngredientSpawnData> currentIngredients = levelData[currentLevelIndex].ingredients;
        if (currentIngredients.Count == 0) return null;

        float totalWeight = currentIngredients.Sum(item => item.spawnWeight);
        if (totalWeight <= 0) return currentIngredients[0].prefab;

        float randomValue = Random.Range(0, totalWeight);

        foreach (var item in currentIngredients)
        {
            if (randomValue <= item.spawnWeight)
            {
                return item.prefab;
            }
            randomValue -= item.spawnWeight;
        }
        return currentIngredients[0].prefab;
    }

    private IEnumerator SpawnIngredientRoutine()
    {
        yield return new WaitForSeconds(currentSpawnInterval);
        
        while (isSpawning)
        {
            GameObject prefabToSpawn = GetRandomPrefabFromList();
            if (prefabToSpawn == null)
            {
                Debug.LogError("No ingredients assigned to spawn for current level: " + currentLevelIndex);
                yield break;
            }

            GameObject newIngredient = Instantiate(prefabToSpawn, transform.position + spawnPositionOffset, Quaternion.identity);

            float newGravity = Mathf.Lerp(initialGravityScale, maxGravityScale, currentNormalizedDifficulty);
            IngredientData data = newIngredient.GetComponent<IngredientData>();
            if (data != null)
            {
                data.SetFallSpeed(newGravity);
            }

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }
    
    public List<IngredientSpawnData> GetCurrentLevelIngredients()
    {
        if (levelData.Count > 0 && currentLevelIndex < levelData.Count)
        {
            return levelData[currentLevelIndex].ingredients;
        }
        return new List<IngredientSpawnData>();
    }
}