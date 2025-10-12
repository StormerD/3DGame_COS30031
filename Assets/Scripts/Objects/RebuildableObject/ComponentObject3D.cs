using UnityEngine;

// yes, this also implements iinteractable... so that i can have access to enter/exit interact zone >:(
[RequireComponent(typeof(Renderer))]
public class ComponentObject3D : ComponentObjectBase, IInteractable
{
    private Material _thisOutlineMaterial;
    void Start()
    {
        foreach (var m in GetComponent<Renderer>().materials)
        {
            // i don't like this, but since the outline is added as an additional material,
            // i'm not sure how to grab specifically it from the renderer's materials without 
            // directly comparing for a name
            if (m.name.ToLower().Contains("outline")) _thisOutlineMaterial = m;
        }
        if (_thisOutlineMaterial == null) Debug.LogError("Failed to locate outline material on object: " + gameObject.name);
    }
    
    public override void Pickup(IInteractor interactor)
    {

    }

    public void EnterInteractZone() => _thisOutlineMaterial.SetFloat("_Enabled", 1);
    public void ExitInteractZone() => _thisOutlineMaterial.SetFloat("_Enabled", 0);
}
