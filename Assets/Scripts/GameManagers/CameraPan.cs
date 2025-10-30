using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public Transform lookTarget;

    void Start()
    {
        // Play main menu music once when the scene loads
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMainMenuMusic();
    }

    void Update()
    {
        if (lookTarget != null)
        {
            transform.RotateAround(lookTarget.position, Vector3.up, rotationSpeed * Time.deltaTime);
            transform.LookAt(lookTarget);
        }
    }

    void OnDisable()
    {
        // Stop the music when leaving the scene
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();
    }
}
