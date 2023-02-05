using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    #region SingletonCode
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    //single pattern ends here
    #endregion

    [Header("Volume")]
    [Range(0, 2)]
    public float masterVolume = 1;
    [Range(0, 2)]
    public float ambienceVolume = 1;
    [Range(0, 2)]
    public float musicVolume = 1;
    [Range(0, 2)]
    public float SFXVolume = 1;

    [Header("Sources")]
    public GameObject player;
    public List<GameObject> campfires = new List<GameObject>();
    public List<GameObject> torches = new List<GameObject>();

    List<EventInstance> sfxEventInstances;

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;

    bool isInWater = false;

    // Start is called before the first frame update
    void Start()
    {
        sfxEventInstances = new List<EventInstance>();

        InitEventInstances();
    }

    public void OnButtonClick()
    {
        RuntimeManager.PlayOneShot(FModEvents.Instance.buttonClick, player.transform.position);
    }

    public void OnBeginRoast()
    {
        RuntimeManager.PlayOneShot(FModEvents.Instance.mushroomRoast, player.transform.position);
    }

    public void OnPickup()
    {
        RuntimeManager.PlayOneShot(FModEvents.Instance.pickup, player.transform.position);
    }

    public void OnEnterWater()
    {
        RuntimeManager.PlayOneShot(FModEvents.Instance.waterEnter, player.transform.position);
        isInWater = true;
    }
    public void OnExitWater()
    {
        isInWater = false;
    }

    void InitEventInstances()
    {
        ambienceEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.ambience);
        RuntimeManager.AttachInstanceToGameObject(ambienceEventInstance, player.transform);
        ambienceEventInstance.start();

        musicEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.music);
        RuntimeManager.AttachInstanceToGameObject(musicEventInstance, player.transform);
        musicEventInstance.start();

        foreach (GameObject campfire in campfires)
        {
            EventInstance newFire = RuntimeManager.CreateInstance(FModEvents.Instance.burningCampfire);
            RuntimeManager.AttachInstanceToGameObject(newFire, campfire.transform);
            sfxEventInstances.Add(newFire);
            newFire.start();
        }

        foreach (GameObject torch in torches)
        {
            EventInstance newTorch = RuntimeManager.CreateInstance(FModEvents.Instance.torch);
            RuntimeManager.AttachInstanceToGameObject(newTorch, torch.transform);
            sfxEventInstances.Add(newTorch);
            newTorch.start();
        }

    }

    // Update is called once per frame
    void Update()
    {
        ambienceEventInstance.setVolume(ambienceVolume * masterVolume);
        musicEventInstance.setVolume(musicVolume * masterVolume);

        foreach (EventInstance fire in sfxEventInstances)
        {
            fire.setVolume(SFXVolume * masterVolume);
        }
    }

    private void OnDestroy()
    {
        ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        foreach (EventInstance fire in sfxEventInstances)
        {
            fire.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}
