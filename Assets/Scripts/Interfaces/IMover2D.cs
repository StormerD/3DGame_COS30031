using UnityEngine;

public interface IMover2D
{
    Vector2 GetCurrentDirection();
    void FreezeActions();
    void UnfreezeActions();
}