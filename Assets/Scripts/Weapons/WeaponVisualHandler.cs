using UnityEngine;

[RequireComponent(typeof(Animator), typeof(IWeapon))]
public class WeaponVisuals : MonoBehaviour
{
    public GameObject visualWrapper;
    private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        IWeapon weaponScript = GetComponent<IWeapon>();
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
