using TMPro;
using UnityEngine;

public class RecipeDisplay : MonoBehaviour
{
    public TextMeshProUGUI recipeTitle;
    public TextMeshProUGUI recipeIngredients;

    int currentRecipe = 0;

    string[] titles = {
        "Vodka Soda",
        "Vodka Cranberry",
        "Martini",
        "Margarita"
    };

    string[] ingredients = {
        "- Vodka\n- Soda\n- Ice",
        "- Vodka\n- Cranberry\n- Ice",
        "- Gin\n- Vermouth",
        "- Tequila\n- Lime\n- Triple Sec"
    };

    void Start()
    {
        ShowRecipe();
    }

    public void NextRecipe()
    {
        currentRecipe++;
        if (currentRecipe >= titles.Length)
            currentRecipe = 0;

        ShowRecipe();
    }

    void ShowRecipe()
    {
        recipeTitle.text = titles[currentRecipe];
        recipeIngredients.text = ingredients[currentRecipe];
    }
}