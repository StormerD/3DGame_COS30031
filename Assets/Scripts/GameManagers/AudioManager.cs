using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton


    private AudioSource audioSource;

    [Header("Footstep Clips")]
    public AudioClip[] footstepClips;


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

    public void PlayFootstep()
    {
        if (footstepClips != null && footstepClips.Length > 0)
        {
            int index = Random.Range(0, footstepClips.Length);
            Debug.Log($"Playing footstep clip {index}");
            audioSource.PlayOneShot(footstepClips[index]);
        }
        else
        {
            Debug.LogWarning("Footstep clips not assigned!");
        }
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
        if (componentPlaced != null)
            audioSource.PlayOneShot(componentPlaced);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
    }
}
