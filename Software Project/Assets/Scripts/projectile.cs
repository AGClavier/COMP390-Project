using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    Rigidbody proj;

    void Start()
    {
        proj = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "wall")
        {
            proj.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
        }
    }
}
