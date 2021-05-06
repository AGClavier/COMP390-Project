using System.Collections;
using UnityEngine;

public class securityCamera : MonoBehaviour
{
    [SerializeField] private UnityStandardAssets.Characters.ThirdPerson.basicDroid ai;
    [SerializeField] private LayerMask layerMask;

    public State state;
    private bool alive;

    public int spot = 0;

    //Variables for investigating
    private float timer = 0;
    private float wait = 4;

    //variables for sight
    public Vector3 destination;
    public GameObject target;
    private float sightDist = 4;
    private float maskSightDist = 8;
    private float lineOfSight = 60;
    private float startingAngle = 90;
    private int raycast = 40;
    RaycastHit hit;

    //variables for line of sight layer
    private Mesh mesh;
    private MeshFilter lOSLayer;
    private Material lOSColour;

    private GameObject[] array;
    private int index;
    private int i = 0;
    private int j = 0;

    public enum State
    {
        PATROL,
        SPOTTED,
    }

    Vector3 RadiansToVector3(float angle)
    {
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }

    Mesh CreateMesh(Vector3[] points)
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

        if (i == 0)
        {
            array = GameObject.FindGameObjectsWithTag("droid");
            index = Random.Range(0, array.Length);
            i = 1;
            j = 1;
        }

        if (j == 1)
        {
            ai = GameObject.FindGameObjectsWithTag("droid")[index].GetComponent<UnityStandardAssets.Characters.ThirdPerson.basicDroid>();
            spot = 1;
        }

        if (timer >= wait)
        {
            state = securityCamera.State.PATROL;
            timer = 0;
            spot = 0;
            i = 0;
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
            angle -= transform.rotation.eulerAngles.y;
            angle += startingAngle;

            if (Physics.Raycast(transform.position, RadiansToVector3(angle * Mathf.Deg2Rad), out hit, maskSightDist/2, layerMask))
            {
                points[i] = hit.point - transform.position;
                //Debug.DrawRay(transform.position, RadiansToVector3(angle * Mathf.Deg2Rad), Color.red);
            }
            else
            {
                points[i] = Vector3.down * 2 + RadiansToVector3(angle * Mathf.Deg2Rad).normalized * maskSightDist;
            }
        }

        lOSLayer.mesh = CreateMesh(points);

        Vector3 dir1 = Quaternion.Euler(0, 7.5f, 0) * transform.forward;
        Vector3 dir2 = Quaternion.Euler(0, 15, 0) * transform.forward;
        Vector3 dir3 = Quaternion.Euler(0, 22.5f, 0) * transform.forward;
        Vector3 dir4 = Quaternion.Euler(0, 30, 0) * transform.forward;
        Vector3 dir5 = Quaternion.Euler(0, 37.5f, 0) * transform.forward;
        Vector3 dir6 = Quaternion.Euler(0, 45, 0) * transform.forward;
        Vector3 dir7 = Quaternion.Euler(0, 52.5f, 0) * transform.forward;
        Vector3 dir8 = Quaternion.Euler(0, -7.5f, 0) * transform.forward;
        Vector3 dir9 = Quaternion.Euler(0, -15, 0) * transform.forward;
        Vector3 dir10 = Quaternion.Euler(0, -22.5f, 0) * transform.forward;
        Vector3 dir11 = Quaternion.Euler(0, -30, 0) * transform.forward;
        Vector3 dir12 = Quaternion.Euler(0, -37.5f, 0) * transform.forward;
        Vector3 dir13 = Quaternion.Euler(0, -45, 0) * transform.forward;
        Vector3 dir14 = Quaternion.Euler(0, -52.5f, 0) * transform.forward;

        Debug.DrawRay(transform.position, transform.forward * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir1).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir2).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir3).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir4).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir5).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir6).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir7).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir8).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir9).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir10).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir11).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir12).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir13).normalized * sightDist, Color.black);
        Debug.DrawRay(transform.position, (transform.forward + dir14).normalized * sightDist, Color.black);

        if (Physics.Raycast(transform.position, transform.forward, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                destination = hit.point;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir1).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                destination = hit.point;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir2).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                destination = hit.point;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir3).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                destination = hit.point;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir4).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                destination = hit.point;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir5).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                destination = hit.point;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir6).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                destination = hit.point;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir7).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                destination = hit.point;
                target = hit.collider.gameObject;
            }
        }

        if (Physics.Raycast(transform.position, (transform.forward + dir8).normalized, out hit, sightDist))
        {
            if (hit.collider.gameObject.tag == "MC")
            {
                state = securityCamera.State.SPOTTED;
                destination = hit.point;
                target = hit.collider.gameObject;
            }
        }
    }
}