using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HoverInfo : MonoBehaviour
{
    [Header("Tooltip Message")]
    public string message;

    [Header("Tooltip Offset Above Object")]
    public Vector3 offset = new Vector3(0, 0.2f, 0);

    private Tooltip _tooltip;

    private void Awake()
    {
        // Auto-subscribe to XR hover events — no manual Inspector wiring needed
        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(_ => OnHoverEnter());
            interactable.hoverExited.AddListener(_ => OnHoverExit());
        }
    }

    private void Start()
    {
        _tooltip = FindFirstObjectByType<Tooltip>();
    }

    public void OnHoverEnter()
    {
        _tooltip?.ShowWorldPosition(message, transform.position + offset);
    }

    public void OnHoverExit()
    {
        _tooltip?.Hide();
    }
}
