using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //change weapon
    public void ChangeWeapon(int weapon)
    {
        animator.SetInteger("WeaponType", weapon);
    }

    [ContextMenu("Attack")]
    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    [ContextMenu("Block")]
    public void Block()
    {
        animator.SetTrigger("Block");
    }

    [ContextMenu("Set fist")]
    public void SetFist()
    {
        ChangeWeapon(0);
    }

    [ContextMenu("Set greatsword")]
    public void SetSword()
    {
        ChangeWeapon(1);
    }

    [ContextMenu("play animation")]
    public void PlayAnimation()
    {
        int action = Random.Range(0, 2);
        if (action == 0)
            Attack();
        else
            Block();
    }

}
