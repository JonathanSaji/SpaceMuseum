using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class SceneTeleporter : MonoBehaviour
{
    [SerializeField] private string targetScene;

    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }

    private void Start()
    {
        GetComponent<XRGrabInteractable>().selectEntered.AddListener(_ => LoadScene());
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(targetScene))
            SceneManager.LoadScene(targetScene);
    }
}
