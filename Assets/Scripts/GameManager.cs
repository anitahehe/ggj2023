using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Asset References")]
    public List<GameObject> ProgressIndicators;
    public GameObject Player;
    public GameObject HomePoint;
    public GameObject GameEndingFadeToWhite;
    
    [Header("Parameters")]
    public int mushroomsEaten = 0;
    public bool GameIsEnding = false;
    public float GameEndingTransitionLength = 5.0f;
    

    public static GameManager instance;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            EatMushrooms(3);
        }
    }

    public void Awake()
    {
        instance = this;
    }
    
    public void Start()
    {
        foreach (var indicator in ProgressIndicators)
        {
            RemoveProgress(indicator);
        }
    }

    public void EatMushrooms(int incomingMushrooms)
    {
        mushroomsEaten += incomingMushrooms;
        if (mushroomsEaten > 0)
        {
            ShowProgress(ProgressIndicators[0]);
        }
        if (mushroomsEaten >= 4)
        {
            ShowProgress(ProgressIndicators[1]);
        }
        if (mushroomsEaten >= 9)
        {
            EndGame();
        }
    }
    
    void ShowProgress(GameObject progressIndicator)
    {
        // TODO: Fade in or something!
        progressIndicator.SetActive(true);
    }

    void RemoveProgress(GameObject progressIndicator)
    {
        // TODO: Fade out or something!
        progressIndicator.SetActive(false);
    }


    void EndGame()
    {
        GameIsEnding = true;
        ShowProgress(ProgressIndicators[2]);
        // TODO: Start ending!
        StartCoroutine(Ascend());
    }

    public void PunishPlayer(int penalty)
    {
        // Decrease the number of mushrooms!
        mushroomsEaten -= penalty;
        if (mushroomsEaten <= 4)
        {
            RemoveProgress(ProgressIndicators[1]);
        }

        if (mushroomsEaten <= 0)
        {
            mushroomsEaten = 0;
            RemoveProgress(ProgressIndicators[0]);
        }
    }

    IEnumerator Ascend()
    {
        Image img =  GameEndingFadeToWhite.GetComponent<Image>();
        for (float i = 0; i <= GameEndingTransitionLength; i += Time.deltaTime)
        {
            img.color = new Color(1, 1, 1, i / GameEndingTransitionLength);
            yield return null;
        }
        SceneManager.LoadScene("EndScene");
    }
}
