using UnityEngine;

public class ForgeMenuHandler : MonoBehaviour
{
    public void CloseForge()
    {
        ForgeManager.instance.CloseForgeMenu();
    }
}
