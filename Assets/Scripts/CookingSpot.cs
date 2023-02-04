using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CookingSpot : MonoBehaviour
{
    [Header("Asset References")]
    public List<GameObject> fireObjects;
    public List<GameObject> mushroomObjects;
    public Material cookedMushroom;
    public Skewer PlayerSkewer;
    
    [Header("Parameters")]
    public Vector3 fireOffset;
    public float cookTimer = 5.0f;
    public float cookTimeRangeLower = 1.0f;
    public float cookTimeRangeUpper = 5.0f;

    bool _isCooking = false;
    bool _isFeeding = false;
    

    void Start()
    {
        foreach (var fire in fireObjects)
        {
            fire.SetActive(false);
        }
    }
    
    void Update()
    {
        if (_isCooking)
        {
            for (int i = 0; i < mushroomObjects.Count; i++)
            {
                fireObjects[i].transform.position = mushroomObjects[i].transform.position + fireOffset;
            }
        }
    }

    void StartCooking()
    {
        Debug.Log("start cooking");
        PlayerInput.all[0].currentActionMap.Disable();

        _isCooking = true;
        foreach (var fire in fireObjects)
        {
            fire.SetActive(true);
        }
        StartCoroutine(BurningTimer());
    }

    void OnBurningEnd()
    {
        _isCooking = false;
        foreach (var fire in fireObjects)
        {
            fire.SetActive(false);
        }

        foreach (var mushroom in mushroomObjects)
        {
            foreach (var meshRenderer in mushroom.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.material = cookedMushroom;
            }
            mushroom.GetComponentInChildren<Mushroom>().Cooked = true;
        }
        mushroomObjects.Clear();
        StartCoroutine(AcceptanceTimer());
    }
    

    private void OnFeedingEnd()
    {
        foreach (GameObject mushroom in PlayerSkewer.MushroomsSkewered)
        {
            // TODO: Check if poisinous and act accordingly, otherwise progress game.
            Destroy(mushroom);
        }
        PlayerSkewer.MushroomsSkewered.Clear();
        PlayerInput.all[0].currentActionMap.Enable();
        // TODO: Return camera position.
    }
    
    IEnumerator BurningTimer()
    {
        yield return new WaitForSeconds(cookTimer);
        OnBurningEnd();
    }
    
    IEnumerator AcceptanceTimer()
    {
        //TODO: add screenshake and stuff
        yield return new WaitForSeconds(2f);
        OnFeedingEnd();
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerSkewer = other.transform.parent.parent.gameObject.GetComponent<Skewer>();
        if (other.CompareTag("Mushroom") && !_isCooking && PlayerSkewer.GatherSkewer && (PlayerSkewer.MushroomsSkewered.Count == 3))
        {
            foreach (GameObject skeweredMushroom in PlayerSkewer.MushroomsSkewered)
            {
                mushroomObjects.Add(skeweredMushroom);
            }
            StartCooking();
        }
    }
}
