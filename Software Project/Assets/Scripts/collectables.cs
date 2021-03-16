using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectables : MonoBehaviour
{
    public GameObject key;
    public GameObject chip;
    private GameObject keyInstantiate;
    private GameObject chipInstantiate;
    public GameObject[] keyLocations;
    public GameObject[] chipLocations;

    private int spawnPoint1;
    private int spawnPoint2;
    private int spawnPoint3;

    void Start()
    {
        keyLocations = GameObject.FindGameObjectsWithTag("kLocations");
        spawnPoint1 = Random.Range(0, keyLocations.Length);

        keyInstantiate = GameObject.Instantiate(key, keyLocations[spawnPoint1].transform.position, Quaternion.identity, transform);
        
        chipLocations = GameObject.FindGameObjectsWithTag("cLocations");
        spawnPoint2 = Random.Range(0, chipLocations.Length - 2);
        spawnPoint3 = Random.Range(2, chipLocations.Length);

        chipInstantiate = GameObject.Instantiate(chip, chipLocations[spawnPoint2].transform.position, Quaternion.identity, transform);
        chipInstantiate = GameObject.Instantiate(chip, chipLocations[spawnPoint3].transform.position, Quaternion.identity, transform);
    }
}
