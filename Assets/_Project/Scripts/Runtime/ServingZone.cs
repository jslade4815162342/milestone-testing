using UnityEngine;

/// <summary>
/// Place on a trigger collider on the bar counter.
/// When the player sets a glass down here, GameManager evaluates the drink.
///
/// SETUP:
///   1. Create an empty child on the bar counter.
///   2. Add Box Collider → Is Trigger ✓.
///   3. Add this component.
///   4. (Optional) assign a flat quad as Highlight Visual — it glows green while
///      waiting and yellow while evaluating.
/// </summary>
public class ServingZone : MonoBehaviour
{
    [Header("Visual (optional)")]
    public GameObject highlightVisual;
    public Color idleColor      = new Color(0.2f, 1f, 0.2f, 0.4f);
    public Color evaluateColor  = new Color(1f, 1f, 0.2f, 0.4f);

    private Renderer _highlight;

    private void Awake()
    {
        if (highlightVisual != null)
            _highlight = highlightVisual.GetComponent<Renderer>();
        SetColor(idleColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        var glass = other.GetComponent<DrinkGlass>();
        if (glass == null) return;

        SetColor(evaluateColor);
        GameManager.Instance?.OnDrinkServed(glass);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<DrinkGlass>() != null)
            SetColor(idleColor);
    }

    private void SetColor(Color c)
    {
        if (_highlight == null) return;
        var mat = _highlight.material;
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
        else mat.color = c;
    }
}
