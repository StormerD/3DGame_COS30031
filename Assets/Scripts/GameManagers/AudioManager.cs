using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton


    private AudioSource audioSource;

    [Header("Clips")]
    public AudioClip dashClip;
    public AudioClip attackClip;
    public AudioClip purchaseErrorSound;
    public AudioClip purchaseSuccessSound;
    public AudioClip EquipItemSound;
    public AudioClip componentPlaced;

    private void Awake()
    {
        // Singleton pattern (only one AudioManager allowed)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persist between scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPurchaseSuccess()
    {
        if (purchaseSuccessSound != null)
            audioSource.PlayOneShot(purchaseSuccessSound);
    }
    public void PlayDashSound()
    {
        if (dashClip != null)
            audioSource.PlayOneShot(dashClip);
    }

    public void PlayPurchaseError()
    {
        if (purchaseErrorSound != null)
            audioSource.PlayOneShot(purchaseErrorSound);
    }
    public void PlayEquipItemSound()
    {
        if (EquipItemSound != null)
            audioSource.PlayOneShot(EquipItemSound);
    }
    public void PlayAttackSound()
    {
        if (attackClip != null)
            audioSource.PlayOneShot(attackClip);
    }
    
    public void PlayComponentPlaced()
    {
        if (attackClip != null)
            audioSource.PlayOneShot(componentPlaced);
    }
}
