using UnityEngine;

[RequireComponent(typeof(Animator), typeof(WeaponBase))]
public class WeaponVisuals : MonoBehaviour
{
    public GameObject visualWrapper;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        WeaponBase weaponScript = GetComponent<WeaponBase>();
        weaponScript.OnBasicUsedReady += OnAttack;
        weaponScript.OnSecondaryUsedReady += OnSecondary;
    }

    void OnAttack(Vector2 v)
    {
        if (v != Vector2.zero) visualWrapper.transform.up = v;
        _animator.SetTrigger("Attack");
    }
    void OnSecondary(Vector2 v)
    {
        if (v != Vector2.zero) visualWrapper.transform.up = v;
        _animator.SetTrigger("Secondary");
    }

}
