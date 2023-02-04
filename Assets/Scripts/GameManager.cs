using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Asset References")]
    public List<GameObject> ProgressIndicators;
    public GameObject Player;
    [Header("Parameters")]
    public static int NUM_GOAL_MUSHROOMS = 9;
    public int mushroomsEaten = 0;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            EatMushrooms(3);
        }
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
        if (mushroomsEaten >= NUM_GOAL_MUSHROOMS / 2)
        {
            ShowProgress(ProgressIndicators[1]);
        }
        if (mushroomsEaten >= NUM_GOAL_MUSHROOMS)
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
        ShowProgress(ProgressIndicators[2]);
        // TODO: Start ending!
        StartCoroutine(Ascend()); // TEMPORARY FLIGHT INTO SKY
    }

    void PunishPlayer(int penalty)
    {
        // Decrease the number of mushrooms!
        mushroomsEaten -= penalty;
        if (mushroomsEaten < NUM_GOAL_MUSHROOMS / 2)
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
        while (true)
        {
            Player.transform.position += Vector3.up;
            yield return true;
        }
    }
}
