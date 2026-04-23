using System.Text;
using TMPro;
using UnityEngine;

public class RecipeDisplay : MonoBehaviour
{
    public TextMeshProUGUI recipeTitle;
    public TextMeshProUGUI recipeIngredients;

    private void Start() => Refresh();

    public void Refresh()
    {
        var recipe = RecipeManager.Instance?.Current;
        if (recipe == null) { SetText("—", "No recipe loaded."); return; }

        if (recipeTitle != null) recipeTitle.text = recipe.drinkName;

        if (recipeIngredients != null)
        {
            var sb = new StringBuilder();
            foreach (var ingredient in recipe.requiredIngredients)
                sb.AppendLine($"• {FormatName(ingredient.ToString())}");
            recipeIngredients.text = sb.ToString().TrimEnd();
        }
    }

    private static string FormatName(string raw)
    {
        var sb = new StringBuilder();
        foreach (char c in raw)
        { if (char.IsUpper(c) && sb.Length > 0) sb.Append(' '); sb.Append(c); }
        return sb.ToString();
    }

    private void SetText(string title, string body)
    {
        if (recipeTitle != null) recipeTitle.text = title;
        if (recipeIngredients != null) recipeIngredients.text = body;
    }

    // Keep NextRecipe wired to the button — delegates to RecipeManager
    public void NextRecipe() => RecipeManager.Instance?.NextRecipe();
}
