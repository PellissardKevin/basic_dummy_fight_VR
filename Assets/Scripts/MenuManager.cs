using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject Menu_Login;
    public Animator menuAnimator;
    private float delayBeforeDeactivation = 4.0f;


    // Start is called before the first frame update
    void Start()
    {
        Menu_Login.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void menu_Up()
    {
        menuAnimator.SetTrigger("Anim_Up");

        StartCoroutine(DeactivateMenuWithDelay());
    }

        IEnumerator DeactivateMenuWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeDeactivation);

        Menu_Login.SetActive(false);
    }
}
