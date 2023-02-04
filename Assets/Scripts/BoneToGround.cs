using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneToGround : MonoBehaviour
{
    public GameObject groundTarget;
    Vector3 lastPointHit;
   
  
    void FixedUpdate()
    {
        int layerMask = 1 << 6;


        RaycastHit hit;
        if (Physics.Raycast(transform.position,-transform.up,out hit,Mathf.Infinity,layerMask))
        {
            lastPointHit = hit.point;
           groundTarget.transform.position = new Vector3(groundTarget.transform.position.x, hit.point.y, groundTarget.transform.position.z);
            Debug.Log("hit" + hit.collider.gameObject.name) ;
        }

        Debug.DrawLine(transform.position, lastPointHit, Color.magenta);
    }
}
