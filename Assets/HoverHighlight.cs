using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;

    public Color highlightColor = Color.yellow;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    public void OnHoverEnter()
    {
        rend.material.color = highlightColor;
    }

    public void OnHoverExit()
    {
        rend.material.color = originalColor;
    }
}