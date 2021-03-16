using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class character : MonoBehaviour
{
    public UIManager ui;

    Vector3 original_pos;
    Rigidbody mc;
    Rigidbody proj;
    Rigidbody cloak;
    GameObject prefab1;
    GameObject prefab2;
    GameObject door;

    private float timerStart = 0;
    private float timerStop = 4;
    private float timerStart2 = 0;
    private float timerStop2 = 2;
    private int ammo = 0;
    private float normalSpeed = 0;
    private float hiddenSpeed = 25;

    void Start()
    {
        mc = GetComponent<Rigidbody>();

        prefab1 = Resources.Load("Projectile") as GameObject;
        prefab2 = Resources.Load("Cloak") as GameObject;
        door = Resources.Load("Door1") as GameObject;

        original_pos = new Vector3(mc.transform.position.x, mc.transform.position.y, mc.transform.position.z);

        ui = GameObject.FindWithTag("ui").GetComponent<UIManager>();
    }

    void OnTriggerEnter(Collider col)
    {
        //When the user walks to the exit they will be taken to the next level
        if (col.tag == "exit1")
        {
            SceneManager.LoadScene("Stage2");
        }

        if (col.tag == "exit2")
        {
            SceneManager.LoadScene("Stage3");
        }

        if (col.tag == "exit3")
        {
            SceneManager.LoadScene("Victory");
        }

        if (col.tag == "Start")
        {
            Destroy(GameObject.FindWithTag("Door"));
        }

        if (col.tag == "key")
        {
            Destroy(col.gameObject);
            Destroy(GameObject.FindWithTag("Door2"));
        }

        //When the user picks up a chip, the data is sent to the "UIManager" script and their score is increased
        if (col.tag == "chip")
        {
            Destroy(col.gameObject);
            ui.IncrementScore();
        }

        //If the user is caught by an enemy they are sent back to the beginning (original_pos) and the lose of life data is again sent to "UIManager"
        if (col.tag == "droid")
        {
            mc.transform.position = original_pos;
            ui.IncrementLives();
        }
    }

    void Update()
    {
        timerStart += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && ammo == 0)
        {
            GameObject Projectile = Instantiate(prefab1) as GameObject;
            Projectile.transform.position = transform.position + Vector3.up * 0.5f + transform.forward;
            proj = Projectile.GetComponent<Rigidbody>();
            proj.velocity = transform.forward * 9;

            ammo++;
        }

        if (GameObject.FindWithTag("projectile") != null)
        {
            if (timerStart >= timerStop)
            {
                Destroy(GameObject.FindWithTag("projectile"));
                timerStart = 0;

                ammo--;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            gameObject.tag = "MCHidden";
            mc.drag = hiddenSpeed;
        }

        if (Input.GetMouseButtonUp(1))
        {
            gameObject.tag = "MC";
            mc.drag = normalSpeed;
        }

        if (GameObject.FindWithTag("Door") == null)
        {
            timerStart2 += Time.deltaTime;

            if (timerStart2 >= timerStop2)
            {
                Instantiate(door, new Vector3(-1, 1, -10), Quaternion.identity);
                timerStart2 = 0;
            }
        }
    }
}