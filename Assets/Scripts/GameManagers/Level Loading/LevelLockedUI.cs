using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Animator))]
public class LevelLockedUI : MonoBehaviour
{
    private Animator levelLock;

    void Awake()
    {
        levelLock = GetComponent<Animator>();
    }
    public void SetUnlocked(bool to)
    {
        levelLock.SetBool("Unlocked", to);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) levelLock.SetTrigger("PlayerEnteredZone");
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) levelLock.SetTrigger("PlayerExitedZone");
    }
}
