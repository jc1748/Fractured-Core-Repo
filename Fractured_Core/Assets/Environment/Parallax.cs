using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float parallaxSpeed = 0.5f;
    private Transform cam;
    private Vector3 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cam.position - lastCamPos;
        transform.position += new Vector3(delta.x * parallaxSpeed, 0, 0);
        lastCamPos = cam.position;
    }
}