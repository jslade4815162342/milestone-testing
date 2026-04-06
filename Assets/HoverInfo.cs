using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; 

public class HoverInfo : MonoBehaviour
{
    [Header("Tooltip Message")]
    public string message;

    [Header("Tooltip Offset Above Object")]
    public Vector3 offset = new Vector3(0, 0.2f, 0);

    private Tooltip tooltip;

    void Start()
    {
        tooltip = FindObjectOfType<Tooltip>();
    }

   
    public void OnHoverEnter()
    {
        if (tooltip != null)
        {
            tooltip.ShowWorldPosition(message, transform.position + offset);
        }
    }


    public void OnHoverExit()
    {
        if (tooltip != null)
        {
            tooltip.Hide();
        }
    }
}