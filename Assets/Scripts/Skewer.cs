using System.Collections.Generic;
using UnityEngine;

    public class Skewer : MonoBehaviour
{
    public SphereCollider DetectionSphere;
    public GameObject ForagableMushroom;
    public List<GameObject> SkewerSpots;
    [field: SerializeField] public bool GatherSkewer { get; private set; }
    public HashSet<GameObject> MushroomsSkewered;
    void Start()
    {
        MushroomsSkewered = new HashSet<GameObject>();
    }
    
    void Update()
    {
        // Find Mushrooms near me!
        ForagableMushroom = null;
        var detected = Physics.OverlapSphere(transform.position, DetectionSphere.radius);
        List<GameObject> mushroomsNearMe = new List<GameObject>();
        foreach (var hitCollider in detected)
        {
            if (hitCollider.gameObject.tag == "Mushroom" && !MushroomsSkewered.Contains(hitCollider.gameObject))
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
        
        // TODO: Add interaction check for skewering...
        if ((ForagableMushroom != null) && GatherSkewer)
        {
            TryCollectMushroom();
        }
        
    }

    void TryCollectMushroom()
    {
        if (ForagableMushroom is null || MushroomsSkewered.Count >= SkewerSpots.Count || MushroomsSkewered.Contains(ForagableMushroom))
            return;
        GameObject mushroom = ForagableMushroom;
        ForagableMushroom = null;
        mushroom.transform.position = SkewerSpots[MushroomsSkewered.Count].transform.position;
        mushroom.transform.parent = SkewerSpots[MushroomsSkewered.Count].transform;
        AudioManager.Instance.OnPickup();
        MushroomsSkewered.Add(mushroom);
    }
    
    public void ToggleSkewer()
    {
        GatherSkewer = !GatherSkewer;

        if (GatherSkewer)
        {
            transform.Rotate(0,0,60f);
        }
        else
        {
            transform.Rotate(0,0,-60f);
        }
    }

    public void ClearSkewer()
    {
        foreach (GameObject mushroom in MushroomsSkewered)
        {
            Destroy(mushroom);
        }
        MushroomsSkewered.Clear();
    }

    public void TeleportHome()
    {
        GameManager.instance.Player.SetActive(false);
        GameManager.instance.Player.transform.position = GameManager.instance.HomePoint.transform.position;
        GameManager.instance.Player.SetActive(true);
    }
}
