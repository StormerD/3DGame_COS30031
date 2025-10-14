using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class InteractableOutline3D : MonoBehaviour
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

    public void EnterInteractZone() => _thisOutlineMaterial.SetFloat("_Enabled", 1);
    public void ExitInteractZone() { _thisOutlineMaterial.SetFloat("_Enabled", 0); RemoveInteractFocus(); }
    private void ChangeOutlineColor(Color color) => _thisOutlineMaterial.SetColor("_OutlineColor", color);
    public void SetInteractFocus(Color color) => ChangeOutlineColor(color);
    public void RemoveInteractFocus() => ChangeOutlineColor(new(1, 1, 1));
}
