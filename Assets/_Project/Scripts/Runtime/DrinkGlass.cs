using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add to every glass in the scene.
///
/// Tracks which ingredients have been poured in and shows a simple colour-fill
/// cylinder as a stand-in for liquid (no physics simulation needed).
///
/// SETUP:
///   1. Add this component to the glass GameObject.
///   2. Add XRGrabInteractable so the player can pick it up.
///   3. Add a Trigger Collider (sphere, r ≈ 0.12) centred just above the rim.
///      This detects when a bottle is held above the glass.
///   4. (Optional) create a child Cylinder inside the glass, set Y scale to 0,
///      and assign it to Liquid Visual. Give it a URP/Lit material.
/// </summary>
public class DrinkGlass : MonoBehaviour
{
    [Header("Capacity")]
    [Tooltip("How many ingredient pours the glass holds before it is full.")]
    public int maxIngredients = 5;

    [Header("Liquid Visual (optional)")]
    [Tooltip("Child cylinder that grows as liquid is added.")]
    public Transform liquidVisual;
    [Tooltip("Max local Y scale of the cylinder when glass is full.")]
    public float maxLiquidScale = 0.08f;

    // ── Public state ──────────────────────────────────────────────────────────
    public List<IngredientType> Ingredients { get; private set; } = new();
    public bool IsFull => Ingredients.Count >= maxIngredients;

    private Renderer _liquidRenderer;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        // Ensure all attached colliders are triggers
        foreach (var col in GetComponents<Collider>())
            col.isTrigger = true;

        if (liquidVisual != null)
        {
            _liquidRenderer = liquidVisual.GetComponent<Renderer>();
            var s = liquidVisual.localScale;
            s.y = 0f;
            liquidVisual.localScale = s;
        }
    }

    // ── Bottle detection ──────────────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        var bottle = other.GetComponent<BottleInteractable>();
        bottle?.SetGlass(this);
    }

    private void OnTriggerExit(Collider other)
    {
        var bottle = other.GetComponent<BottleInteractable>();
        bottle?.ClearGlass();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Pour one ingredient into the glass. Ignored if full.</summary>
    public void AddIngredient(IngredientType type)
    {
        if (IsFull) return;
        Ingredients.Add(type);
        UpdateVisual();
        BarAudioManager.Instance?.PlayClink();
        GameManager.Instance?.OnGlassUpdated(this);
        Debug.Log($"[DrinkGlass] {type} added ({Ingredients.Count}/{maxIngredients})");
    }

    /// <summary>Check whether this glass satisfies the given recipe.</summary>
    public bool Satisfies(DrinkRecipe recipe) =>
        recipe != null && recipe.IsSatisfiedBy(Ingredients);

    /// <summary>Empty the glass and reset visuals (called between rounds).</summary>
    public void Clear()
    {
        Ingredients.Clear();
        UpdateVisual();
    }

    // ── Visual ────────────────────────────────────────────────────────────────
    private void UpdateVisual()
    {
        if (liquidVisual == null) return;

        float ratio = maxIngredients > 0 ? (float)Ingredients.Count / maxIngredients : 0f;
        var s = liquidVisual.localScale;
        s.y = Mathf.Lerp(0f, maxLiquidScale, ratio);
        liquidVisual.localScale = s;

        if (_liquidRenderer != null && Ingredients.Count > 0)
        {
            Color col = GetIngredientColor(Ingredients[^1]);
            var mat = _liquidRenderer.material;
            if (mat.HasProperty(BaseColorID)) mat.SetColor(BaseColorID, col);
            else mat.color = col;
        }
    }

    private static Color GetIngredientColor(IngredientType t) => t switch
    {
        IngredientType.Vodka          => new Color(0.9f, 0.9f, 1.0f, 0.6f),
        IngredientType.Rum            => new Color(0.6f, 0.3f, 0.1f, 0.8f),
        IngredientType.Gin            => new Color(0.8f, 0.9f, 0.8f, 0.6f),
        IngredientType.Tequila        => new Color(1.0f, 0.95f, 0.5f, 0.7f),
        IngredientType.Whiskey        => new Color(0.7f, 0.4f, 0.1f, 0.85f),
        IngredientType.TripleSec      => new Color(1.0f, 0.8f, 0.3f, 0.7f),
        IngredientType.LimeJuice      => new Color(0.5f, 0.9f, 0.2f, 0.8f),
        IngredientType.LemonJuice     => new Color(1.0f, 0.95f, 0.2f, 0.8f),
        IngredientType.OrangeJuice    => new Color(1.0f, 0.55f, 0.1f, 0.85f),
        IngredientType.CranberryJuice => new Color(0.8f, 0.05f, 0.15f, 0.85f),
        IngredientType.SodaWater      => new Color(0.9f, 0.95f, 1.0f, 0.4f),
        IngredientType.GrenadineSyrup => new Color(0.9f, 0.1f, 0.2f, 0.85f),
        IngredientType.BlueCuracao    => new Color(0.1f, 0.35f, 0.9f, 0.8f),
        _                             => new Color(0.5f, 0.5f, 0.5f, 0.7f),
    };
}
