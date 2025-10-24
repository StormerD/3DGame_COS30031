using UnityEngine;

public class MenuCameraOrbit : MonoBehaviour
{
    [Header("Orbit Settings")]
    public Transform target;      // The object the camera orbits around
    public float distance = 15f;  // Distance from the target
    public float orbitSpeed = 10f; // Degrees per second

    [Header("Camera Height")]
    public float heightOffset = 2f; // Y offset from the target

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("MenuCameraOrbit: No target set! Please assign a target Transform.");
        }

        // Initial position
        if (target != null)
        {
            Vector3 dir = (transform.position - target.position).normalized;
            transform.position = target.position + dir * distance;
            transform.position = new Vector3(transform.position.x, target.position.y + heightOffset, transform.position.z);
            transform.LookAt(target.position + Vector3.up * heightOffset);
        }
    }

    void Update()
    {
        if (target == null) return;

        // Orbit horizontally around target
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);

        // Maintai
    }
}