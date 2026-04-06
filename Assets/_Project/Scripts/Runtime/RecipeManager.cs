using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the list of DrinkRecipe assets and tracks which one the player is
/// currently working on.
///
/// NOTE: Your teammate's RecipeDisplay.cs handles the on-screen text separately.
/// RecipeManager drives the *game logic* (current recipe, validation, cycling).
/// Wire them together by calling RecipeDisplay.ShowRecipe() from here if desired,
/// or leave them independent — both will work for M3.
///
/// SETUP:
///   1. Add to the GameManager GameObject.
///   2. Drag DrinkRecipe ScriptableObject assets into the Recipes list.
/// </summary>
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    [Header("Recipes")]
    [Tooltip("Drag DrinkRecipe assets here. Create them via right-click → Create → Bartending → Drink Recipe.")]
    public List<DrinkRecipe> recipes = new();

    [Tooltip("Randomise recipe order each round.")]
    public bool shuffle = false;

    public DrinkRecipe Current { get; private set; }
    public int TotalCompleted  { get; private set; }

    private int _index = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (recipes.Count == 0)
        {
            Debug.LogWarning("[RecipeManager] No recipes assigned — add DrinkRecipe assets in the Inspector.");
            return;
        }
        NextRecipe();
    }

    /// <summary>Advance to the next recipe and refresh the UI.</summary>
    public void NextRecipe()
    {
        if (recipes.Count == 0) return;

        if (shuffle)
        {
            // Avoid repeating the same recipe twice in a row
            int next = _index;
            if (recipes.Count > 1)
                while (next == _index)
                    next = Random.Range(0, recipes.Count);
            _index = next;
        }
        else
        {
            _index = (_index + 1) % recipes.Count;
        }

        Current = recipes[_index];
        Debug.Log($"[RecipeManager] New recipe: {Current.drinkName}");

        // Keep the canvas in sync with the current recipe
        FindFirstObjectByType<RecipeDisplay>()?.Refresh();
    }

    /// <summary>
    /// Returns true if the glass satisfies the current recipe.
    /// Increments TotalCompleted on success.
    /// </summary>
    public bool Validate(DrinkGlass glass)
    {
        if (Current == null || glass == null) return false;
        bool ok = glass.Satisfies(Current);
        if (ok) TotalCompleted++;
        return ok;
    }
}
