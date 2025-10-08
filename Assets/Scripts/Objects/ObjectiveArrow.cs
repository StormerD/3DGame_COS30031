using UnityEngine;

public class ObjectiveArrow : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform target;
    public float heightOffset = 1.5f;

    [Header("Pulse Settings")]
    public bool enablePulse = true;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.2f;
    public float stopPulseDistance = 1f;

    private Vector3 _baseScale;

    void Start()
    {
        _baseScale = transform.localScale;

        if (player == null && transform.parent != null)
            player = transform.parent;
    }

    void LateUpdate()
    {
        if (player == null || target == null) return;

        // Move above player
        transform.position = player.position + Vector3.up * heightOffset;

        // Distance to target
        float dist = Vector2.Distance(player.position, target.position);

        // Hide arrow if very close
        if (dist <= 0.1f)
        {
            gameObject.SetActive(false);
            return;
        }
        else if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        // Rotate to point at target
        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        // Pulse effect
        if (enablePulse && dist > stopPulseDistance)
        {
            float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = _baseScale * (1 + scaleOffset);
        }
        else
        {
            transform.localScale = _baseScale;
        }
    }
}
