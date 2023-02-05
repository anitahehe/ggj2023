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
            //Destroy(this.gameObject);
            _instance = this;
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

    List<EventInstance> sfxEventInstances = new List<EventInstance>();

    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    private EventInstance moveLandEventInstance;
    private EventInstance moveWaterEventInstance;

    bool _isInWater = false;
    bool _isMoving = false;
    bool _wasMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        InitEventInstances();
    }

    public void UpdateIsMoving(bool isMoving)
    {
        _isMoving = isMoving;
    }

    public void SwitchMoving()
    {
        if (_isInWater)
        {
            moveLandEventInstance.setPaused(true);
            moveWaterEventInstance.setPaused(false);
        }
        else
        {
            moveLandEventInstance.setPaused(false);
            moveWaterEventInstance.setPaused(true);
        }
    }

    public void OnButtonClick()
    {
        RuntimeManager.PlayOneShot(FModEvents.Instance.buttonClick, player.transform.position);
    }
    public void OnChew()
    {
        RuntimeManager.PlayOneShot(FModEvents.Instance.Chewing, player.transform.position);
    }

    public void OnPageFlip()
    {
        RuntimeManager.PlayOneShot(FModEvents.Instance.pageFlip, player.transform.position);
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
        _isInWater = true;
        SwitchMoving();
    }
    public void OnExitWater()
    {
        _isInWater = false;
        SwitchMoving();
    }

    void InitEventInstances()
    {
        ambienceEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.ambience);
        RuntimeManager.AttachInstanceToGameObject(ambienceEventInstance, player.transform);
        ambienceEventInstance.start();

        musicEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.music);
        RuntimeManager.AttachInstanceToGameObject(musicEventInstance, player.transform);
        musicEventInstance.start();

        moveLandEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.MoveLand);
        RuntimeManager.AttachInstanceToGameObject(moveLandEventInstance, player.transform);
        moveLandEventInstance.start();
        moveLandEventInstance.setPaused(true);
        moveWaterEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.MoveWater);
        RuntimeManager.AttachInstanceToGameObject(moveWaterEventInstance, player.transform);
        moveWaterEventInstance.start();
        moveWaterEventInstance.setPaused(true);

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
        if (_isMoving != _wasMoving)
        {
            if (_isInWater)
            {
                moveWaterEventInstance.setPaused(!_isMoving);
            }
            else
            {
                moveLandEventInstance.setPaused(!_isMoving);
            }
            _wasMoving = _isMoving;
        }

        ambienceEventInstance.setVolume(ambienceVolume * masterVolume);
        musicEventInstance.setVolume(musicVolume * masterVolume);
        moveLandEventInstance.setVolume(SFXVolume * masterVolume);
        moveWaterEventInstance.setVolume(SFXVolume * masterVolume);

        foreach (EventInstance fire in sfxEventInstances)
        {
            fire.setVolume(SFXVolume * masterVolume);
        }
    }

    private void OnDestroy()
    {
        //ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        moveLandEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        moveWaterEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        foreach (EventInstance fire in sfxEventInstances)
        {
            fire.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}
