using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public Transform lookTarget;

    void Update()
    {
        if (lookTarget != null)
        {
            transform.RotateAround(lookTarget.position, Vector3.up, rotationSpeed * Time.deltaTime);
            transform.LookAt(lookTarget);
        }
    }
}
