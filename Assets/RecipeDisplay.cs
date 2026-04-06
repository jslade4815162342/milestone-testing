using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Drives the world-space recipe canvas.
/// Pulls the current recipe directly from RecipeManager so the display
/// always stays in sync with the game state.
///
/// RecipeManager calls Refresh() automatically each time the recipe advances.
/// </summary>
public class RecipeDisplay : MonoBehaviour
{
    public TextMeshProUGUI recipeTitle;
    public TextMeshProUGUI recipeIngredients;

    private void Start() => Refresh();

    /// <summary>Updates the canvas to show RecipeManager.Current.</summary>
    public void Refresh()
    {
        var recipe = RecipeManager.Instance?.Current;

        if (recipe == null)
        {
            SetText("—", "No recipe loaded.\nAdd DrinkRecipe assets\nto RecipeManager.");
            return;
        }

        if (recipeTitle != null)
            recipeTitle.text = recipe.drinkName;

        if (recipeIngredients != null)
        {
            var sb = new StringBuilder();
            foreach (var ingredient in recipe.requiredIngredients)
                sb.AppendLine($"• {FormatName(ingredient.ToString())}");
            recipeIngredients.text = sb.ToString().TrimEnd();
        }
    }

    // Splits CamelCase into readable words: OrangeJuice → Orange Juice
    private static string FormatName(string raw)
    {
        var sb = new StringBuilder();
        foreach (char c in raw)
        {
            if (char.IsUpper(c) && sb.Length > 0)
                sb.Append(' ');
            sb.Append(c);
        }
        return sb.ToString();
    }

    private void SetText(string title, string body)
    {
        if (recipeTitle != null)       recipeTitle.text       = title;
        if (recipeIngredients != null) recipeIngredients.text = body;
    }
}
