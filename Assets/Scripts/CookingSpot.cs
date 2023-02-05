using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class CookingSpot : MonoBehaviour
{
    [Header("Asset References")]
    public List<GameObject> fireObjects;
    public List<GameObject> mushroomObjects;
    public Material cookedMushroom;
    public Skewer PlayerSkewer;
    public CinemachineVirtualCamera CookingCamera;
    public CinemachineImpulseSource ImpulseSource;
    [SerializeField] public SkinnedMeshRenderer SlugIdolMR;
    
    [Header("Parameters")]
    public Vector3 fireOffset;
    public float cookTimer = 5.0f;
    public float cookTimeRangeLower = 1.0f;
    public float cookTimeRangeUpper = 5.0f;
    private Color _origSlugIdolColor;
    
    bool _isCooking = false;
    bool _isFeeding = false;


    void Awake()
    {
        ImpulseSource = GetComponent<CinemachineImpulseSource>();
    }
    void Start()
    {
        _origSlugIdolColor = SlugIdolMR.material.color;

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
    void ScreenShake()
    {
        ImpulseSource.GenerateImpulse();
    }

    void StartCooking()
    {
        Debug.Log("start cooking");
        PlayerInput.all[0].currentActionMap.Disable();
        CookingCamera.Priority = 20;
        AudioManager.Instance.OnBeginRoast();

        _isCooking = true;
        for (int i = 0; i < mushroomObjects.Count; i++)
        {
            fireObjects[i].SetActive(true);
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
        AudioManager.Instance.OnChew();
        StartCoroutine(AcceptanceTimer());
    }
    

    private void OnFeedingEnd()
    {
        int poisonedSkewer = 0;
        foreach (GameObject mushroom in PlayerSkewer.MushroomsSkewered)
        {
            // TODO: Check if poisonous and act accordingly, otherwise progress game.
            if (mushroom.GetComponent<Mushroom>().Poisinous)
            {
                poisonedSkewer++;
            }
        }

        if (poisonedSkewer > 0)
        {
            SlugIdolMR.material.color = Color.red;
            ScreenShake();
            GameManager.instance.PunishPlayer(3);
        }
        else
        {
            SlugIdolMR.material.color = Color.green;
            GameManager.instance.EatMushrooms(3);
        }
        
        foreach (GameObject mushroom in PlayerSkewer.MushroomsSkewered)
        {
            Destroy(mushroom);
        }
        PlayerSkewer.MushroomsSkewered.Clear();

        StartCoroutine(FinishFoodTimer());

    }
    
    IEnumerator BurningTimer()
    {
        yield return new WaitForSeconds(cookTimer);
        OnBurningEnd();
    }
    
    IEnumerator AcceptanceTimer()
    {
        yield return new WaitForSeconds(3f);
        OnFeedingEnd();
    }
    
    IEnumerator FinishFoodTimer()
    {
        yield return new WaitForSeconds(3f);
        SlugIdolMR.material.color = _origSlugIdolColor;
        if (!GameManager.instance.GameIsEnding)
        {
            PlayerInput.all[0].currentActionMap.Enable();
            CookingCamera.Priority = 0;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        other.transform.parent.parent.gameObject.TryGetComponent<Skewer>(out PlayerSkewer);
        if (other.gameObject.name.Contains("Player"))
        {
            return;
        }
        if (other.CompareTag("Mushroom") && !_isCooking && PlayerSkewer.GatherSkewer && PlayerSkewer.MushroomsSkewered.Count == 3)
        {
            foreach (GameObject skeweredMushroom in PlayerSkewer.MushroomsSkewered)
            {
                mushroomObjects.Add(skeweredMushroom);
            }
            StartCooking();
        }
    }
}
