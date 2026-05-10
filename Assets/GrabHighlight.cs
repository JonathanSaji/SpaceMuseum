using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabHighlight : MonoBehaviour
{
    public Color highlightColor = Color.yellow;
    private Material mat;
    private Color originalColor;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;

    void Start()
    {
        mat = GetComponentInChildren<Renderer>().material;
        originalColor = mat.color;
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        interactable.hoverEntered.AddListener(OnHover);
        interactable.hoverExited.AddListener(OnHoverExit);
    }

    void OnHover(HoverEnterEventArgs args)
    {
        mat.color = highlightColor;
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        mat.color = originalColor;
    }
}