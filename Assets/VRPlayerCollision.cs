using UnityEngine;
using UnityEngine.XR;

public class VRPlayerCollision : MonoBehaviour
{
    private CharacterController controller;
    public Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Keep the CharacterController centered on the headset
        Vector3 headPos = cameraTransform.localPosition;
        Vector3 currentCenter = controller.center;
        controller.center = new Vector3(headPos.x, currentCenter.y, headPos.z);

        // Apply gravity manually
        controller.Move(Vector3.down * 2f * Time.deltaTime);
    }
}