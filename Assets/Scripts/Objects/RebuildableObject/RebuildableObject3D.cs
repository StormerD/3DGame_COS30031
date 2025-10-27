using UnityEngine;

// fyi: Only difference between this and 2d version is the RequireComponent, hence the empty class
[RequireComponent(typeof(Collider))]
public class RebuildableObject3D : RebuildableObjectBase, IInteractable
{ }