using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Booklet : MonoBehaviour
{


    Animator bookAnimator;

    // Start is called before the first frame update
    void Start()
    {
        bookAnimator = this.GetComponent<Animator>();
    }

    private void OnMouseOver()
    {

        bookAnimator.SetBool("onHover", true);
        Debug.Log("mouse over");
    }

    private void OnMouseExit()
    {
        bookAnimator.SetBool("onHover", false);
        Debug.Log("mouse gone");
    }
}
