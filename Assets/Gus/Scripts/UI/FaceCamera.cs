using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // LateUpdate garantit que la caméra a fini de bouger avant la rotation
        transform.forward = cam.forward;
    }
}