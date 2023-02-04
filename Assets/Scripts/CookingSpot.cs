using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSpot : MonoBehaviour
{
    [Header("Asset References")]
    public List<GameObject> fireObjects;
    public List<GameObject> mushroomObjects;
    public Material cookedMushroom;

    [Header("Parameters")]
    public Vector3 fireOffset;
    public float cookTimer = 5.0f;

    bool _isCooking = false;
    
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
        }
        
        mushroomObjects.Clear();
    }

    IEnumerator BurningTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(cookTimer);
            OnBurningEnd();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mushroom") && !_isCooking)
        {
            foreach (GameObject skeweredMushroom in FindObjectOfType<Skewer>().MushroomsSkewered)
            {
                mushroomObjects.Add(skeweredMushroom);
            }
            StartCooking();
        }
    }
}
