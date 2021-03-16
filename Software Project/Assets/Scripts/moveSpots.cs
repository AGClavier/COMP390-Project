using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveSpots : MonoBehaviour
{
    public GameObject moveSpot1;
    public GameObject moveSpot2;

    private GameObject moveSpotInstantiate;
    public GameObject[] moveSpotLocations;

    private int spawnPoint1;
    private int spawnPoint2;
    private int spawnPoint3;
    private int spawnPoint4;

    void Start()
    {
        moveSpotLocations = GameObject.FindGameObjectsWithTag("Movespot");
        spawnPoint1 = Random.Range(0, moveSpotLocations.Length);
        spawnPoint2 = Random.Range(0, moveSpotLocations.Length);
        spawnPoint3 = Random.Range(0, moveSpotLocations.Length);
        spawnPoint4 = Random.Range(0, moveSpotLocations.Length);

        moveSpotInstantiate = GameObject.Instantiate(moveSpot1, moveSpotLocations[spawnPoint1].transform.position, Quaternion.identity, transform);
        moveSpotInstantiate = GameObject.Instantiate(moveSpot1, moveSpotLocations[spawnPoint2].transform.position, Quaternion.identity, transform);
        moveSpotInstantiate = GameObject.Instantiate(moveSpot1, moveSpotLocations[spawnPoint3].transform.position, Quaternion.identity, transform);
        moveSpotInstantiate = GameObject.Instantiate(moveSpot1, moveSpotLocations[spawnPoint4].transform.position, Quaternion.identity, transform);
        
        moveSpotInstantiate = GameObject.Instantiate(moveSpot2, moveSpotLocations[spawnPoint1].transform.position, Quaternion.identity, transform);
        moveSpotInstantiate = GameObject.Instantiate(moveSpot2, moveSpotLocations[spawnPoint2].transform.position, Quaternion.identity, transform);
        moveSpotInstantiate = GameObject.Instantiate(moveSpot2, moveSpotLocations[spawnPoint3].transform.position, Quaternion.identity, transform);
        moveSpotInstantiate = GameObject.Instantiate(moveSpot2, moveSpotLocations[spawnPoint4].transform.position, Quaternion.identity, transform);
    }
}
