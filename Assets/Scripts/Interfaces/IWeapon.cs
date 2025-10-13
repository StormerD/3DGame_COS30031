

using System;
using UnityEngine;

public interface IWeapon
{
    event Action OnBasicReady;
    event Action<Vector2> OnBasicUsedReady;
    event Action OnBasicUsedNotReady;
    event Action OnSecondaryReady;
    event Action<Vector2> OnSecondaryUsedReady;
    event Action OnSecondaryUsedNotReady;
    void Attack();
    void Secondary();
    WeaponData GetWeaponData();
    void LinkNewMover(IMover2D mover);
}