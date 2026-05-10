using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class VideoTriggerZone : MonoBehaviour
{
    [SerializeField] private VideoExperienceManager manager;

    private void Start()
    {
        GetComponent<SphereCollider>().isTrigger = true; // override what you see in the inspector; this is a trigger.
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detect the player’s CharacterController capsule.
        if (other.GetComponent<CharacterController>() != null)
            manager.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
            manager.Stop();
    }
}
