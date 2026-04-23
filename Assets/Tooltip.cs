using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    [Header("Text Component")]
    public TextMeshProUGUI tooltipText;

    [Header("Optional Camera to Face")]
    public Camera mainCamera;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Hide();
    }

    public void ShowWorldPosition(string message, Vector3 worldPosition)
    {
        gameObject.SetActive(true);
        tooltipText.text = message;

        transform.position = worldPosition;

        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180f, 0); 
        }
    }

    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}