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
    public GameObject campfire;

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    private EventInstance campfireEventInstance;

    // Start is called before the first frame update
    void Start()
    {
        InitEventInstances();
    }

    void InitEventInstances()
    {
        ambienceEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.ambience);
        RuntimeManager.AttachInstanceToGameObject(ambienceEventInstance, player.transform);
        ambienceEventInstance.start();

        musicEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.music);
        RuntimeManager.AttachInstanceToGameObject(musicEventInstance, player.transform);
        musicEventInstance.start();

        campfireEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.burningCampfire);
        RuntimeManager.AttachInstanceToGameObject(campfireEventInstance, campfire.transform);
        campfireEventInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        ambienceEventInstance.setVolume(ambienceVolume * masterVolume);
        musicEventInstance.setVolume(musicVolume * masterVolume);
        campfireEventInstance.setVolume(SFXVolume * masterVolume);

    }

    private void OnDestroy()
    {
        ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        campfireEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
