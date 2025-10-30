using UnityEngine;

public class WorldChanger : MonoBehaviour
{
    [SerializeField] private BasicEventObject triggersTransition;
    [SerializeField] private GameObject destroyedWorldTilemap;
    [SerializeField] private GameObject destroyedWorldProps;
    [SerializeField] private GameObject healedWorldTilemap;
    [SerializeField] private GameObject healedWorldProps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.instance != null && GameManager.instance.GetHasActiveLevelBeenCompleted())
        {
            HealWorld();
        }
        else
        {
            triggersTransition.RegisterListener(HealWorld);
        }
    }

    void OnDestroy()
    {
        triggersTransition.UnregisterListener(HealWorld);
    }
    
    private void HealWorld()
    {

        Debug.Log("Healing world!");
        if (destroyedWorldTilemap != null) destroyedWorldTilemap.SetActive(false);
        if (destroyedWorldProps != null) destroyedWorldProps.SetActive(false);
        if (healedWorldTilemap != null) healedWorldTilemap.SetActive(true);
        if (healedWorldProps != null) healedWorldProps.SetActive(true);

        // Play nature sounds
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayNatureAmbience();
        }



        // destroy enemies
        var spawner = FindFirstObjectByType<SpawnManager>();
        if (spawner != null)
        {
            spawner.StopAndClear(destroy: true);
        }

    }
}
