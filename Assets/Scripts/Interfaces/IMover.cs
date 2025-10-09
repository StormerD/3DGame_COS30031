using UnityEngine;

public interface IMover
{
    Vector2 GetCurrentDirection();
    void FreezeActions();
    void UnfreezeActions();
}