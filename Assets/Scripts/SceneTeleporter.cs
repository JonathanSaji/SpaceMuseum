using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class SceneTeleporter : MonoBehaviour
{
    [SerializeField] private string targetScene;

    private void Awake()
    {
        // Collider must NOT be a trigger for grab to work.
        var col = GetComponent<SphereCollider>();
        col.isTrigger = false;

        // Kinematic rigidbody — no gravity, no physics knock-away.
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // XR Grab Interactable — match every interaction layer so the
        // controllers' Ray and Direct interactors can always find us.
        var grab = GetComponent<XRGrabInteractable>();
        grab.interactionLayers = -1; // all layers
        grab.movementType = XRBaseInteractable.MovementType.Kinematic;
    }

    private void Start()
    {
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(_ =>
        {
            if (!string.IsNullOrEmpty(targetScene))
                SceneManager.LoadScene(targetScene);
        });
    }
}
