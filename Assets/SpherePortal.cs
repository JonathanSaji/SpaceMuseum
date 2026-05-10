using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;

public class SpherePortal : MonoBehaviour
{
    [Header("Hover Effect")]
    public float hoverScaleMultiplier = 1.2f;
    public float hoverSpeed = 5f;
    public Color hoverColor = new Color(0f, 0.8f, 1f);

    [Header("Input")]
    public InputActionReference aButtonReference; // assign in Inspector

    [Header("UI Prompt")]
    public GameObject pressAPrompt; // assign a world-space UI canvas/text

    private XRGrabInteractable grab;
    private Vector3 originalScale;
    private Color originalColor;
    private Material mat;
    private bool isHovered = false;

    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();

        originalScale = transform.localScale;
        mat = GetComponent<Renderer>().material;
        originalColor = mat.color;

        grab.hoverEntered.AddListener(OnHoverEnter);
        grab.hoverExited.AddListener(OnHoverExit);

        if (pressAPrompt != null)
            pressAPrompt.SetActive(false);
    }

    void Update()
    {
        // Smooth scale
        Vector3 targetScale = isHovered
            ? originalScale * hoverScaleMultiplier
            : originalScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale, targetScale, Time.deltaTime * hoverSpeed);

        // Check A button only while hovered
        if (isHovered && aButtonReference != null)
        {
            if (aButtonReference.action.WasPressedThisFrame())
                SceneManager.LoadScene(1);
        }
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        isHovered = true;
        mat.color = hoverColor;

        if (pressAPrompt != null)
            pressAPrompt.SetActive(true);
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        isHovered = false;
        mat.color = originalColor;

        if (pressAPrompt != null)
            pressAPrompt.SetActive(false);
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.hoverEntered.RemoveListener(OnHoverEnter);
            grab.hoverExited.RemoveListener(OnHoverExit);
        }
    }
}