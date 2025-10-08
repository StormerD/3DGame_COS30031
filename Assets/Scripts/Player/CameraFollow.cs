using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float dampTime = 0.1f; 

    private Vector3 currentVelocity; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!player) Debug.LogError("Camera is missing player reference!");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), ref currentVelocity, dampTime);
    }
}
