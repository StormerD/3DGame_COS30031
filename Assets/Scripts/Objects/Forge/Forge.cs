using System;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class Forge : MonoBehaviour, IInteractable
{
    public event Action OnForgeOpen;

    void Start()
    {
        OnForgeOpen += ForgeManager.instance.OpenForgeMenu;
        OnForgeOpen += LevelManager.instance.ForgeOpened;
    }

    public void Interact(IInteractor interactor)
    {
        OnForgeOpen?.Invoke();
    }
}
                           