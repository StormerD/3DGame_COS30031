using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement; // for scene change detection


[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton

    [SerializeField] private AudioSource masterAudio;
    [SerializeField] private AudioSource sfxAudio;
    [SerializeField] private AudioSource musicAudio;
    [SerializeField] private AudioSource ambientAudio;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip NatureAmbience;


    [Header("Mixing")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private KeyedValueEventObject audioValueChanged;
    [SerializeField] private KeyedValueEventObject audioGroupsInitializationStream;
    [SerializeField] private BasicEventObject requestEmitAudioValues;
    [SerializeField] private float volumeThreshold = 0.0001f;
    [SerializeField] private float exampleAudioCooldown = 0.5f;
    [SerializeField] private float adjustmentDiff = 0.001f;
    private Dictionary<string, float> setVols = new();

    [Header("Footstep Clips")]
    public AudioClip[] footstepClips;


    [Header("Clips")]
    public AudioClip dashClip;
    public AudioClip attackClip;
    public AudioClip purchaseErrorSound;
    public AudioClip purchaseSuccessSound;
    public AudioClip EquipItemSound;
    public AudioClip componentPlaced;
    [SerializeField] private AudioClip itemPickedUp;

    private bool recentlyPlayedExample = false;
    private bool overridingAllSounds = false;

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
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // add scene listener
        requestEmitAudioValues.RegisterListener(EmitRequestRecieved);
        audioValueChanged.RegisterListener(SetMixerGroupVolume);
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // remove listener
        requestEmitAudioValues.UnregisterListener(EmitRequestRecieved);
        audioValueChanged.UnregisterListener(SetMixerGroupVolume);
    }

    void Start()
    {
        if (GameManager.instance != null) GameManager.instance.OnLoadComplete += SyncAudioOverrides;
    }

    private void SyncAudioOverrides()
    {
        Dictionary<string, float> test = GameManager.instance.GetAudioOverrides();
        overridingAllSounds = true;
        foreach (var k in test.Keys)
        {
            SetMixerGroupVolume(k, test[k]);
        }
        overridingAllSounds = false;
    }

    public void SetMixerGroupVolume(string group, float volume)
    {
        volume = Mathf.Clamp(volume, volumeThreshold, 1);
        if (setVols.ContainsKey(group) && Mathf.Abs(setVols[group] - volume) < adjustmentDiff)
        {
            Debug.Log("change minimal; ignoring");
            return;
        }
        setVols[group] = volume;
        mixer.SetFloat(group, Mathf.Log10(volume) * 20);
        if (!overridingAllSounds) PlayExample(group);
    }

    private void EmitRequestRecieved()
    {
        foreach (var kv in setVols)
        {
            audioGroupsInitializationStream.RaiseEvent(kv.Key, kv.Value);
        }
    }

    public Dictionary<string, float> GetAudioOverrides() => setVols;

    public float GetGroupVolume(string group) => setVols.GetValueOrDefault(group, 1);

    public void PlayFootstep()
    {
        if (footstepClips != null && footstepClips.Length > 0)
        {
            int index = Random.Range(0, footstepClips.Length);
            sfxAudio.PlayOneShot(footstepClips[index]);
        }
        else
        {
            Debug.LogWarning("Footstep clips not assigned!");
        }
    }

    public void PlayNatureAmbience()
    {
        if (NatureAmbience != null)
        {
            // Stop any existing ambient audio first
            if (ambientAudio.isPlaying)
            ambientAudio.Stop();

            // Assign the nature clip and play it
            ambientAudio.clip = NatureAmbience;
            ambientAudio.loop = true;
            ambientAudio.Play();
        }
        else
        {
            Debug.LogWarning("NatureAmbience clip not assigned in AudioManager!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop all audio if not in Level1
        if (scene.name != "Level1")
        {
            StopMusic();
            if (ambientAudio.isPlaying)
                ambientAudio.Stop();
        }

        // Play MainMenu music
        if (scene.name == "MainMenu")
        {
            StopMusic();
            if (ambientAudio.isPlaying)
                ambientAudio.Stop();
            PlayMainMenuMusic();
        }

        // Play nature ambience when entering HomeScene
        if (scene.name == "HomeArea")
        {
            StopMusic(); // optional
            PlayNatureAmbience();
        }
    }



    public void StopMusic()
    {
        if (musicAudio.isPlaying)
            musicAudio.Stop();
    }


   public void PlayMainMenuMusic()
{
        if (mainMenuMusic != null)
        {
            // Stop any ambient audio first
            if (ambientAudio.isPlaying)
                ambientAudio.Stop();

            musicAudio.clip = mainMenuMusic;
            musicAudio.loop = true; // keeps it playing
            musicAudio.Play();
        }
        else
        {
            Debug.LogWarning("Main menu music clip not assigned!");
        }
    }


    public void PlayPurchaseSuccess()
    {
        if (purchaseSuccessSound != null)
            sfxAudio.PlayOneShot(purchaseSuccessSound);
    }
    public void PlayDashSound()
    {
        if (dashClip != null)
            sfxAudio.PlayOneShot(dashClip);
    }

    public void PlayPurchaseError()
    {
        if (purchaseErrorSound != null)
            sfxAudio.PlayOneShot(purchaseErrorSound);
    }
    public void PlayEquipItemSound()
    {
        if (EquipItemSound != null)
            sfxAudio.PlayOneShot(EquipItemSound);
    }
    public void PlayAttackSound()
    {
        if (attackClip != null)
            sfxAudio.PlayOneShot(attackClip);
    }

    public void PlayComponentPlaced()
    {
        if (componentPlaced != null)
            sfxAudio.PlayOneShot(componentPlaced);
    }

    public void PlayItemPickedUp()
    {
        if (itemPickedUp != null)
            sfxAudio.PlayOneShot(itemPickedUp);
    }

    public void PlayAudioClip(AudioClip clip)
    {
        if (clip != null) masterAudio.PlayOneShot(clip);
    }

    private void PlayExample(string group)
    {
        if (recentlyPlayedExample) return;
        recentlyPlayedExample = true;

        // play an example based on the group inputted
        switch(group)
        {
            case "AmbientVolume":
                Debug.Log("playing amb");
                ambientAudio.PlayOneShot(componentPlaced);
                break;
            case "SFXVolume":
                Debug.Log("playing sfx");
                sfxAudio.PlayOneShot(componentPlaced);
                break;
        }
        StartCoroutine(CooldownExampleSound());
    }
    
    private IEnumerator CooldownExampleSound()
    {
        yield return new WaitForSeconds(exampleAudioCooldown);
        recentlyPlayedExample = false;
    }
}
