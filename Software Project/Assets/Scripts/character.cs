using UnityEngine;
using UnityEngine.SceneManagement;

public class character : MonoBehaviour
{
    public uiManager ui;

    Vector3 original_pos;
    Rigidbody mc;
    Rigidbody projectile;
    GameObject proj;
    GameObject door;
    GameObject child;

    Renderer mcColour;
    Color colour1;
    Color colour2;

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

        proj = Resources.Load("Projectile") as GameObject;
        door = Resources.Load("Door1") as GameObject;

        Color colour1 = new Color(0.1f, 0.5f, 1);
        Color colour2 = new Color(0.1f, 0.5f, 1);

        original_pos = new Vector3(mc.transform.position.x, mc.transform.position.y, mc.transform.position.z);

        ui = GameObject.FindWithTag("ui").GetComponent<uiManager>();
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

        if (col.tag == "randomDroid")
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
            GameObject Projectile = Instantiate(proj) as GameObject;
            Projectile.transform.position = transform.position + Vector3.up * 0.5f + transform.forward;
            projectile = Projectile.GetComponent<Rigidbody>();
            projectile.velocity = transform.forward * 9;

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

            foreach (Renderer mcColour in GetComponentsInChildren<Renderer>())
            {
                mcColour.material.color = new Color(0.75f, 0.84f, 1); ;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            gameObject.tag = "MC";
            mc.drag = normalSpeed;

            foreach (Renderer mcColour in GetComponentsInChildren<Renderer>())
            {
                mcColour.material.color = new Color(0.19f, 0.5f, 1); ;
            }
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