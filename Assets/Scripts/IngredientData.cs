using UnityEngine;

/// <summary>
/// This component holds the essential data for an ingredient.
/// We attach this to each ingredient PREFAB.
/// </summary>
public class IngredientData : MonoBehaviour
{
    [Header("Ingredient Info")]
    [Tooltip("A unique ID for this ingredient, e.g., 'Auge' or 'Pilz'.")]
    public string ingredientID;
    
    [Tooltip("The number of points this ingredient is worth.")]
    public int scoreValue;

    // We can add more data here later, like:
    // public Sprite recipeIcon;
    // public AudioClip correctSound;
}