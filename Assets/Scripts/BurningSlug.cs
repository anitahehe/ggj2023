using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningSlug : MonoBehaviour
{
    [Header("Asset References")]
    public GameObject fireObject;
    public GameObject slug;
    public SkinnedMeshRenderer slugMesh;
    public Material charredSlug;

    [Header("Parameters")]
    public Vector3 fireOffset;
    public float burnTimer = 5.0f;

    bool _isBurning = false;

    // Start is called before the first frame update
    void Start()
    {
        fireObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isBurning)
        {
            fireObject.transform.position = slug.transform.position + fireOffset;
        }
    }

    void StartBurning()
    {
        _isBurning = true;
        fireObject.SetActive(true);
        StartCoroutine(BurningTimer());
    }

    void OnBurningEnd()
    {
        _isBurning = false;
        fireObject.SetActive(false);
        slugMesh.material = charredSlug;
    }

    IEnumerator BurningTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(burnTimer);
            OnBurningEnd();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !_isBurning)
        {
            StartBurning();
        }
    }
}
