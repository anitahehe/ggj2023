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


    public GameObject player;

    public List<EventInstance> eventInstances;

    private EventInstance ambienceEventInstance;

    // Start is called before the first frame update
    void Start()
    {
        eventInstances = new List<EventInstance>();

        ambienceEventInstance = RuntimeManager.CreateInstance(FModEvents.Instance.ambience);
        RuntimeManager.AttachInstanceToGameObject(ambienceEventInstance, player.transform);
        ambienceEventInstance.start();
    }

    // Update is called once per frame
    void Update()
    {
        ambienceEventInstance.setVolume(ambienceVolume * masterVolume);

    }

    private void OnDestroy()
    {
        ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
