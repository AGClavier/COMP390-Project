using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class character : MonoBehaviour
{
    public UIManager ui;

    Vector3 original_pos;
    Rigidbody mc;

    void Start()
    {
        mc = GetComponent<Rigidbody>();
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

        if (col.tag == "key")
        {
            Destroy(col.gameObject);
            Destroy(GameObject.FindWithTag("Door"));
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
}
