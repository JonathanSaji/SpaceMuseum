using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private bool loadOnEnter = true;

    private void Start()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!loadOnEnter) return;
        if (other.GetComponent<CharacterController>() != null)
            SceneManager.LoadScene(targetScene);
    }

    private void OnTriggerExit(Collider other)
    {
        if (loadOnEnter) return;
        if (other.GetComponent<CharacterController>() != null)
            SceneManager.LoadScene(targetScene);
    }
}
