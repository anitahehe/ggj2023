using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skewer : MonoBehaviour
{
    public SphereCollider DetectionSphere;

    public GameObject ForagableMushroom;

    private HashSet<GameObject> MushroomsSkewered;

    void Update()
    {
        // Find Mushrooms near me!
        ForagableMushroom = null;
        var detected = Physics.OverlapSphere(transform.position, DetectionSphere.radius);
        List<GameObject> mushroomsNearMe = new List<GameObject>();
        foreach (var hitCollider in detected)
        {
            if (hitCollider.gameObject.tag == "Mushroom")
            {
                // I have a mushroom near me!
                mushroomsNearMe.Add(hitCollider.gameObject);
            }
        }
        float closestSqrDist = 0f;
        foreach(var mushroom in mushroomsNearMe)
        {
            float sqrDist = (gameObject.transform.position - transform.position).sqrMagnitude; //sqrMagnitude because it's faster to calculate than magnitude
    
            if (!ForagableMushroom || sqrDist < closestSqrDist) {
                ForagableMushroom = mushroom;
                closestSqrDist = sqrDist;
            }
        }
    }
}
