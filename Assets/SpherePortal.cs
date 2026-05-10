using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;

public class SpherePortal : MonoBehaviour
{
    [Header("Scene")]
    public string sceneToLoad;

    [Header("Hover Effect")]
    public float hoverScaleMultiplier = 1.2f;
    public float hoverSpeed = 5f;
    public Color hoverColor = new Color(0f, 0.8f, 1f);

    [Header("Input")]
    public InputActionReference aButtonReference;

    [Header("UI Prompt")]
    public GameObject pressAPrompt;

    private XRGrabInteractable grab;
    private Vector3 originalScale;
    private Color originalColor;
    private Material mat;
    private bool isHovered = false;

    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();

        if (grab == null)
        {
            Debug.LogError("SpherePortal: No XRGrabInteractable found on " + gameObject.name);
            return;
        }

        originalScale = transform.localScale;

        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend == null)
        {
            Debug.LogError("SpherePortal: No Renderer found on or in children of " + gameObject.name);
            return;
        }

        mat = rend.material;
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
            {
                if (!string.IsNullOrEmpty(sceneToLoad))
                    SceneManager.LoadScene(sceneToLoad);
                else
                    Debug.LogWarning("SpherePortal: sceneToLoad is empty on " + gameObject.name);
            }
        }
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        isHovered = true;
        if (mat != null)
            mat.color = hoverColor;

        if (pressAPrompt != null)
            pressAPrompt.SetActive(true);
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        isHovered = false;
        if (mat != null)
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