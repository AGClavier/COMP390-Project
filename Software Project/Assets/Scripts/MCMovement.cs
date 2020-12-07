using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCMovement : MonoBehaviour {
    Vector3 original_pos;
    Rigidbody MC;

    void Start() {
        MC = GetComponent<Rigidbody>();
        original_pos = new Vector3(MC.transform.position.x, MC.transform.position.y, MC.transform.position.z);
    }

    void Update() {
        float speed = 1.5f;
        Vector3 pos = transform.position;

        if (Input.GetKey("w")) {
            pos.z += speed * Time.deltaTime;
        }
        if (Input.GetKey("s")) {
            pos.z -= speed * Time.deltaTime;
        }
        if (Input.GetKey("d")) {
            pos.x += speed * Time.deltaTime;
        }
        if (Input.GetKey("a")) {
            pos.x -= speed * Time.deltaTime;
        }

        transform.position = pos;
    }

    void OnTriggerEnter(Collider col) {
        if (col.tag == "droid") {
            MC.transform.position = original_pos;
        }
    }
}
