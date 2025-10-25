using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class LevelLockedUIBase : MonoBehaviour
{
    private Animator levelLock;

    protected virtual void Awake()
    {
        levelLock = GetComponent<Animator>();
        LevelSceneChanger controller = GetComponentInParent<LevelSceneChanger>();
        if (controller == null) Debug.LogWarning($"{gameObject.name} unable to find LevelSceneChanger in parent.");
        else
        {
            controller.OnUnlockedChanged += SetUnlocked;
        }
    }
    
    private void SetUnlocked(bool to)
    {
        levelLock.SetBool("Unlocked", to);
    }
    protected void PlayerEnteredTrigger(GameObject other)
    {
        if (IsPlayer(other)) levelLock.SetTrigger("PlayerEnteredZone");
    }
    protected void PlayerExitedTrigger(GameObject other)
    {
        if (IsPlayer(other)) levelLock.SetTrigger("PlayerExitedZone");
    }
    private bool IsPlayer(GameObject test) => test.CompareTag("Player");
}