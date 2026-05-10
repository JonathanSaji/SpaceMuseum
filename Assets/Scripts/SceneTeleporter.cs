using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private float radius = 2f;

    private Transform _head;
    private bool _triggered;

    private void Start()
    {
        if (TryGetComponent<SphereCollider>(out var col))
        {
            col.isTrigger = true;
            col.radius = radius;
        }

        _head = Camera.main?.transform;
    }

    private void Update()
    {
        if (_triggered || _head == null) return;

        if (Vector3.Distance(transform.position, _head.position) < radius)
            Teleport();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;

        if (other.GetComponent<CharacterController>() != null ||
            other.GetComponentInParent<CharacterController>() != null)
            Teleport();
    }

    private void Teleport()
    {
        if (_triggered) return;
        _triggered = true;
        SceneManager.LoadScene(targetScene);
    }
}
