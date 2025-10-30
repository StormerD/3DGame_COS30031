using System;
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
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null) return;
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.position.x, player.position.y, transform.position.z), ref currentVelocity, dampTime);
    }

    public void SetPlayer(Transform to)
    {
        player = to;
        transform.position = new(player.position.x, player.position.y, transform.position.z);
    }
}
