using UnityEngine;

public class door : MonoBehaviour
{
    GameObject doors;
    private float timerStart = 0;
    private float timerStop = 4;
    Quaternion rotation = Quaternion.Euler(0, 90, 0);

    void Start()
    {
        doors = Resources.Load("Door3") as GameObject;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "DoorSensor")
        {
            Destroy(GameObject.FindWithTag("Door3"));
        }
    }

    void Update()
    {
        if (GameObject.FindWithTag("Door3") == null)
        {
            timerStart += Time.deltaTime;

            if (timerStart >= timerStop)
            {
                Instantiate(doors, new Vector3(-4.1f, 1, -0.5f), rotation);
                timerStart = 0;
            }
        }
    }
}
