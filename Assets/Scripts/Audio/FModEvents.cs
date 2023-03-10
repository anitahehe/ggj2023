using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FModEvents : MonoBehaviour
{
    #region SingletonCode
    private static FModEvents _instance;
    public static FModEvents Instance { get { return _instance; } }
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

    [field: SerializeField] public EventReference ambience { get; private set; }
    [field: SerializeField] public EventReference music { get; private set; }
    [field: SerializeField] public EventReference burningCampfire { get; private set; }
    [field: SerializeField] public EventReference pickup { get; private set; }
    [field: SerializeField] public EventReference mushroomRoast { get; private set; }
    [field: SerializeField] public EventReference waterEnter { get; private set; }
    [field: SerializeField] public EventReference torch { get; private set; }
    [field: SerializeField] public EventReference pageFlip { get; private set; }
    [field: SerializeField] public EventReference Chewing { get; private set; }
    [field: SerializeField] public EventReference MoveLand { get; private set; }
    [field: SerializeField] public EventReference MoveWater { get; private set; }
    [field: SerializeField] public EventReference buttonClick { get; private set; }

}
