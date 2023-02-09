using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityTemplateProjects;

public class Booklet : MonoBehaviour
{
    public bool bookIsOpen;
    public AutoFlip pageFlipper;
    public GameObject bookObject;
    Animator bookAnimator;


    private void Start()
    {
        bookAnimator = bookObject.GetComponent<Animator>();
        bookIsOpen = false;

    }

    private void Update()
    {
        //Set status of whether the book is opened based on the animator status
        if (bookAnimator.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
        {
            bookIsOpen = false;
            GameManager.instance.Player.SetActive(true);
        }
        if (bookAnimator.GetCurrentAnimatorStateInfo(0).IsName("Selected"))
            bookIsOpen = true;

        //Closes book + allows left/right arrow to move
        if (bookIsOpen == true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                EventSystem.current.SetSelectedGameObject(null);
                bookAnimator.SetTrigger("Normal");
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                pageFlipper.FlipLeftPage();

            if (Input.GetKeyDown(KeyCode.RightArrow))
                pageFlipper.FlipRightPage();

            Camera.main.GetComponent<Cinemachine.CinemachineBrain>().enabled = false;
            Camera.main.GetComponent<SimpleCameraController>().enabled = false;
        }

        //Opens book on tab
        if (bookIsOpen == false)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                EventSystem.current.SetSelectedGameObject(bookObject);
                GameManager.instance.Player.SetActive(false);
            }

            Camera.main.GetComponent<Cinemachine.CinemachineBrain>().enabled = true;
            Camera.main.GetComponent<SimpleCameraController>().enabled = true;
        }
    }

   
}

