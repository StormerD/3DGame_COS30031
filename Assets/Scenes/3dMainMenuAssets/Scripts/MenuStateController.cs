using UnityEngine;

public class MenuStateController : MonoBehaviour
{
    public MenuCameraOrbit menuOrbit;  // Drag your MenuCameraOrbit component here

    // Call this when starting gameplay
    public void OnStartGame()
    {
        if(menuOrbit != null)
            menuOrbit.enabled = false;
    }

    // Call this when returning to the menu
    public void OnOpenMenu()
    {
        if(menuOrbit != null)
            menuOrbit.enabled = true;
    }
}
