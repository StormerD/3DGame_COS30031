using UnityEngine;

public interface IMover3D
{
    Vector3 GetCurrentDirection();
    void FreezeActions();
    void UnfreezeActions();
}