using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a single drink recipe.
/// Create via:  right-click Project window → Create → Bartending → Drink Recipe
/// Drag created assets into RecipeManager → Recipes list.
/// </summary>
[CreateAssetMenu(fileName = "NewRecipe", menuName = "Bartending/Drink Recipe")]
public class DrinkRecipe : ScriptableObject
{
    [Header("Info")]
    public string drinkName = "My Drink";

    [TextArea(1, 3)]
    public string description = "";

    [Header("Ingredients")]
    [Tooltip("All ingredients the player must pour. Order does not matter.")]
    public List<IngredientType> requiredIngredients = new();

    [Header("Visual")]
    [Tooltip("Colour tint used for the liquid fill in the glass.")]
    public Color liquidColor = new Color(0.9f, 0.7f, 0.3f, 0.85f);

    /// <summary>Returns true when every required ingredient appears in <paramref name="poured"/>.</summary>
    public bool IsSatisfiedBy(List<IngredientType> poured)
    {
        foreach (var needed in requiredIngredients)
            if (!poured.Contains(needed)) return false;
        return true;
    }
}
