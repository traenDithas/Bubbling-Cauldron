using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns ingredients and controls the movement of the entire pipe assembly.
/// The Glass_Pipe and Pipe_LED should be children of this object in the hierarchy.
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
    
    [Tooltip("A small offset from this object's center to spawn ingredients (usually 0, 0).")]
    [SerializeField] 
    private Vector2 spawnPositionOffset = Vector2.zero; // (0, 0)

    // --- UPDATED MOVEMENT ---
    [Header("Assembly Movement")]
    [Tooltip("The lowest Y-position this Spawner object will move to.")]
    [SerializeField] 
    private float spawnerLowestY = 2.0f; 

    private float currentSpawnInterval;
    private bool isSpawning = false;
    
    // This will store our starting Y position.
    private float spawnerInitialY;
    
    void Start()
    {
        currentSpawnInterval = initialSpawnInterval; 
        
        // Store our own starting Y-position
        spawnerInitialY = this.transform.position.y;

        StartSpawning();
    }

    public void UpdateDifficulty(float normalizedDifficulty)
    {
        // 1. Update Spawn Interval
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, fastestSpawnInterval, normalizedDifficulty);
        Debug.Log("New Spawn Interval: " + currentSpawnInterval);

        // 2. Update THIS OBJECT's Y Position
        float newSpawnY = Mathf.Lerp(spawnerInitialY, spawnerLowestY, normalizedDifficulty);
        
        // Move our entire transform (and all its children)
        this.transform.position = new Vector2(this.transform.position.x, newSpawnY);
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
        while (isSpawning)
        {
            if (ingredientPrefabs.Length == 0)
            {
                Debug.LogError("No ingredient prefabs assigned to the spawner!");
                yield break;
            }

            int randomIndex = Random.Range(0, ingredientPrefabs.Length);
            GameObject randomIngredient = ingredientPrefabs[randomIndex];

            // --- UPDATED SPAWN POINT ---
            // Calculate the spawn point based on our *current* position + the offset
            Vector2 spawnPoint = (Vector2)this.transform.position + spawnPositionOffset;
            Instantiate(randomIngredient, spawnPoint, Quaternion.identity);

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }
}