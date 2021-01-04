using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class securityCamera : MonoBehaviour
{
    [SerializeField] private UnityStandardAssets.Characters.ThirdPerson.basicAI ai;
    [SerializeField] private LayerMask layerMask;

    public State state;
    private bool alive;

    public int spot = 0;

    //Variables for investigating
    private float timer = 0;
    private float wait = 4;

    //variables for sight
    private GameObject target;
    private float sightDist = 8;
    private float lineOfSight = 60;
    private float startingAngle = 90;
    private int raycast = 1024;
    RaycastHit hit;

    private Mesh mesh;
    private MeshFilter lOSLayer;
    private Material lOSColour;

    public enum State
    {
        PATROL,
        SPOTTED,
    }

    Vector3 RadiansToVector3(float angle)
    {
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }

    Mesh CreateMeshFromPoints(Vector3[] points)
    {
        mesh = new Mesh();

        points[0] = Vector3.zero;
        mesh.vertices = points;

        int[] trianglesArray = new int[(mesh.vertices.Length - 1) * 3];
        int count = 1;

        for (int i = 0; i < trianglesArray.Length - 3; i += 3)
        {
            trianglesArray[i] = count;
            trianglesArray[i + 1] = 0;
            trianglesArray[i + 2] = count + 1;

            count++;
        }

        mesh.triangles = trianglesArray;

        mesh.RecalculateNormals();

        return mesh;
    }

    void Start()
    {
        lOSLayer = GetComponent<MeshFilter>();
        lOSColour = GetComponent<MeshRenderer>().material;

        state = securityCamera.State.PATROL;
        alive = true;

        StartCoroutine(FSM());
    }

    //Here I create a finite state machine in which the AI follows
    //When spawned in the AI will go into its PATROL state where they navigate the map according the its choosen destination is
    //If the AI engages with the user they will switch to their CHASE state until the user is out of sight or caught, to which the AI will revert back to the state PATROL 
    IEnumerator FSM()
    {
        while (alive)
        {
            switch (state)
            {
                case State.PATROL:
                    Patrol();
                    break;

                case State.SPOTTED:
                    Spotted();
                    break;
            }
            yield return null;
        }
    }

    void Patrol()
    {
        lOSColour.color = Color.yellow;
    }

    public void Spotted()
    {
        lOSColour.color = Color.red;
        timer += Time.deltaTime;

        spot = 1;

        if (timer >= wait)
        {
            state = securityCamera.State.PATROL;
            timer = 0;
            spot = 0;
        }

        if (spot == 1)
        {
            ai.Help();
        }
    }

    void FixedUpdate()
    {
        Vector3[] points = new Vector3[raycast + 2];

        for (int i = 1; i <= raycast + 1; i++)
        {

            float angle = (((float)(i - 1) / raycast) * lineOfSight);
            angle -= lineOfSight / 2;
            angle += startingAngle;

            if (Physics.Raycast(transform.position, RadiansToVector3(angle * Mathf.Deg2Rad), out hit, sightDist, layerMask))
            {
                points[i] = hit.point - transform.position;

            }
            else
            {
                points[i] = Vector3.down * 2 + RadiansToVector3(angle * Mathf.Deg2Rad).normalized * sightDist;
            }
        }

        lOSLayer.mesh = CreateMeshFromPoints(points);

        Vector3 dir1 = Quaternion.Euler(0, 15, 0) * transform.forward;
        Vector3 dir2 = Quaternion.Euler(0, 22.5f, 0) * transform.forward;
        Vector3 dir3 = Quaternion.Euler(0, 30, 0) * transform.forward;
        Vector3 dir4 = Quaternion.Euler(0, 7.5f, 0) * transform.forward;
        Vector3 dir5 = Quaternion.Euler(0, -15, 0) * transform.forward;
        Vector3 dir6 = Quaternion.Euler(0, -22.5f, 0) * transform.forward;
        Vector3 dir7 = Quaternion.Euler(0, -30, 0) * transform.forward;
        Vector3 dir8 = Quaternion.Euler(0, -7.5f, 0) * transform.forward;

        Debug.DrawRay(transform.position, transform.forward * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir1).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir2).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir3).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir4).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir5).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir6).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir7).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir8).normalized * sightDist, Color.black);

        //Here I am creating three raycasts which work as the AI's vision
        //When the user is caught in a raycast the FSM will kick in and the AI will begin chasing the user, whilst their is no target in sight the AI will go back to patrolling
        if (Physics.Raycast(transform.position, transform.forward, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir1).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir2).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir3).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir4).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir5).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir6).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir7).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir8).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                target = hit.collider.gameObject;
            }
        }
    }
}