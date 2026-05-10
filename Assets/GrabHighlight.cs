using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabHighlight : MonoBehaviour
{
    public Color highlightColor = Color.yellow;
    
    private Material mat;
    private Color originalColor;
    private XRSimpleInteractable interactable;

    void Start()
    {
        mat = GetComponentInChildren<Renderer>().material;
        originalColor = mat.color;
        interactable = GetComponent<XRSimpleInteractable>();

        if (interactable == null)
        {
            Debug.LogError("No XRSimpleInteractable found on " + gameObject.name);
            return;
        }

        interactable.hoverEntered.AddListener(OnHover);
        interactable.hoverExited.AddListener(OnHoverExit);
    }

    void OnHover(HoverEnterEventArgs args)
    {
        Debug.Log("Hovering: " + gameObject.name);
        mat.color = highlightColor;
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        mat.color = originalColor;
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHover);
            interactable.hoverExited.RemoveListener(OnHoverExit);
        }
    }
}