using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSoundManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            AudioManager.Instance.OnEnterWater();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            AudioManager.Instance.OnExitWater();
        }
    }
}
