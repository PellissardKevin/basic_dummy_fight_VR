using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject Menu_Login;
    public GameObject Menu_Principal;
    public Animator menu_login_Animator;
    public Animator menu_principal_Animator;
    private float delayBeforeDeactivation = 3.0f;


    // Start is called before the first frame update
    void Start()
    {
        Menu_Login.SetActive(true);
        Menu_Principal.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void menu_login_Up()
    {
        menu_login_Animator.SetTrigger("Login_Up");
        StartCoroutine(DeactivateMenuLoginWithDelay());
        Menu_Principal.SetActive(true);
    }


    public void menu_principal_up()
    {
        menu_principal_Animator.SetTrigger("Principal_Up");
        StartCoroutine(DeactivateMenuPrincipalWithDelay());
        Menu_Login.SetActive(true);
    }

    IEnumerator DeactivateMenuLoginWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeDeactivation);
        Menu_Login.SetActive(false);
    }

    IEnumerator DeactivateMenuPrincipalWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeDeactivation);
        Menu_Principal.SetActive(false);
    }
}
