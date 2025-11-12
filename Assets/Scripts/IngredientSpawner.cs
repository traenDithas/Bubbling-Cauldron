using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns ingredients. Is attached to the 'Pipe' object.
/// Moves itself (and its children like Pipe_LED) down.
/// The RecipeManager is responsible for starting/stopping this spawner.
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
    private Vector2 spawnPositionOffset = Vector2.zero;

    [Header("Assembly Movement")]
    [Tooltip("The lowest Y-position this Pipe object will move to.")]
    [SerializeField] 
    private float spawnerLowestY = -1.5f; 

    private float currentSpawnInterval;
    private bool isSpawning = false;
    private float spawnerInitialY;

    
    void Start()
    {
        // --- THIS IS THE FIX ---
        // We removed StartSpawning() from here.
        // The RecipeManager will now tell us when to start!

        currentSpawnInterval = initialSpawnInterval; 
        spawnerInitialY = this.transform.position.y;
    }

    public void UpdateDifficulty(float normalizedDifficulty)
    {
        // 1. Update Spawn Interval
        currentSpawnInterval = Mathf.Lerp(initialSpawnInterval, fastestSpawnInterval, normalizedDifficulty);

        // 2. Update THIS OBJECT's Y Position (moves Pipe and any children)
        float newSpawnY = Mathf.Lerp(spawnerInitialY, spawnerLowestY, normalizedDifficulty);
        this.transform.position = new Vector2(this.transform.position.x, newSpawnY);
    }

    public void StartSpawning()
    {
        // This check prevents multiple loops
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
            // This is where my typo was. It is now fixed.
            if (ingredientPrefabs.Length == 0)
            {
                Debug.LogError("No ingredient prefabs assigned to the spawner!");
                yield break;
            }

            int randomIndex = Random.Range(0, ingredientPrefabs.Length);
            GameObject randomIngredient = ingredientPrefabs[randomIndex];
            
            Vector2 spawnPoint = (Vector2)this.transform.position + spawnPositionOffset;
            Instantiate(randomIngredient, spawnPoint, Quaternion.identity);

            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }
}