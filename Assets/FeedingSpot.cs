using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedingSpot : MonoBehaviour
{
    [Header("Asset References")]
    public Skewer PlayerSkewer;
    [Header("Parameters")]
    public float cookTimeRangeLower = 1.0f;
    public float cookTimeRangeUpper = 5.0f;

    bool _isFeeding = false;

    
    IEnumerator AcceptanceTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(cookTimeRangeLower, cookTimeRangeUpper));
            OnFeedingEnd();
        }
    }

    private void OnFeedingEnd()
    {
        foreach (GameObject mushroom in PlayerSkewer.MushroomsSkewered)
        {
            // TODO: Check if poisinous and act accordingly, otherwise progress game.
            Destroy(mushroom);
        }
        PlayerSkewer.MushroomsSkewered.Clear();
        // TODO: Reenable player movement.
        // TODO: Return camera position.
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mushroom") && other.gameObject.GetComponent<Mushroom>().Cooked)
        {
            StartFeeding();
        }
    }

    private void StartFeeding()
    {
        _isFeeding = true;
        // TODO: Disable player movement.
        // TODO: Start camera transition.
        StartCoroutine(AcceptanceTimer());
    }
}
