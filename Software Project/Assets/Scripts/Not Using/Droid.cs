using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Droid : MonoBehaviour
{
    [SerializeField] private LineOfSight lineOfSight;

    Rigidbody droid;
    Vector3 originalPos;
    Vector3 lastPos;

    void Start()
    {
        droid = GetComponent<Rigidbody>();
        originalPos = new Vector3(droid.transform.position.x, droid.transform.position.y, droid.transform.position.z);
        transform.rotation = Quaternion.LookRotation(transform.position - lastPos);
    }

    void Update()
    {
        Vector3 pos = transform.position;
        lastPos = transform.position;

        //transform.position = pos;

        lineOfSight.SetOrigin(pos);
        lineOfSight.SetAimDirection(lastPos);
        
    }
}
