using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// --- Helper classes ---
[System.Serializable]
public class IngredientSpawnData
{
    public GameObject prefab;
    [Tooltip("Relative chance to spawn compared to others in this specific list.")]
    public float spawnWeight = 1.0f;
}

[System.Serializable]
public class LevelIngredientList
{
    public string levelName;

    // --- NEW SETTING ---
    [Tooltip("How many items should be on the recipe scroll for this level?")]
    public int recipeSize = 3; // Default is 3
    // -------------------
    
    [Header("Recipe Items")]
    [Tooltip("Ingredients that appear on the Recipe Scroll.")]
    public List<IngredientSpawnData> ingredients;

    [Header("Special Items (No Recipe)")]
    [Tooltip("Chance (0-1) that a spawn will be a Special item instead of a Recipe item.")]
    [Range(0f, 1f)]
    public float specialSpawnChance = 0.1f; 

    [Tooltip("Extra items (Coins, Bombs, etc.) that spawn but are NOT in the recipe.")]
    public List<IngredientSpawnData> specialIngredients;
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
    private float spawnerInitialY;
    private float currentNormalizedDifficulty = 0f;
    private int currentLevelIndex = 0;

    void Awake()
    {
        spawnerInitialY = transform.position.y;
        currentSpawnInterval = initialSpawnInterval;
        currentNormalizedDifficulty = 0f;
        currentLevelIndex = 0;
    }

    public void UpdateDifficulty(float normalizedDifficulty, int completedLevelCount)
    {
        currentNormalizedDifficulty = normalizedDifficulty;
        
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, fastestSpawnInterval, normalizedDifficulty);
        
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

        LevelIngredientList currentLevel = levelData[currentLevelIndex];

        // 1. Decide: Special or Normal?
        bool spawnSpecial = false;
        if (currentLevel.specialIngredients.Count > 0)
        {
            if (Random.value < currentLevel.specialSpawnChance)
            {
                spawnSpecial = true;
            }
        }

        List<IngredientSpawnData> targetList = spawnSpecial ? currentLevel.specialIngredients : currentLevel.ingredients;

        if (targetList.Count == 0)
        {
            targetList = currentLevel.ingredients;
        }
        if (targetList.Count == 0) return null; 

        // 3. Pick weighted item
        float totalWeight = targetList.Sum(item => item.spawnWeight);
        if (totalWeight <= 0) return targetList[0].prefab;

        float randomValue = Random.Range(0, totalWeight);

        foreach (var item in targetList)
        {
            if (randomValue <= item.spawnWeight)
            {
                return item.prefab;
            }
            randomValue -= item.spawnWeight;
        }
        return targetList[0].prefab;
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

    // --- NEW FUNCTION ---
    /// <summary>
    /// Helper to get the recipe size for the current level
    /// </summary>
    public int GetCurrentLevelRecipeSize()
    {
        if (levelData.Count > 0 && currentLevelIndex < levelData.Count)
        {
            return levelData[currentLevelIndex].recipeSize;
        }
        return 3; // Default safety value
    }
    // --------------------
}