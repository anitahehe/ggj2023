using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Booklet : MonoBehaviour
{
    public AutoFlip pageFlipper;
    public GameObject bookObject;
    Button bookControl;
    Animator bookAnimator;
    public bool bookIsOpen;

    private void Start()
    {
        bookControl = bookObject.GetComponent<Button>();
        bookAnimator = bookObject.GetComponent<Animator>();

        bookIsOpen = false;
    }

    private void Update()
    {
        if (bookAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
            bookIsOpen = false;

        if (bookAnimator.GetCurrentAnimatorStateInfo(0).IsName("Selected"))
            bookIsOpen = true;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (bookIsOpen == false)
            {
                EventSystem.current.SetSelectedGameObject(bookObject);
            }

            if (bookIsOpen == true)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

        }




    }

   
}

