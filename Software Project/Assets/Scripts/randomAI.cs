using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace UnityStandardAssets.Characters.ThirdPerson
{

    public class randomAI : MonoBehaviour
    {
        //I am using a LayerMask to stop the Ai from being able to look through walls
        public LayerMask mask;
        //I am using a NavMeshAgent to create a moveable area for the AI
        private NavMeshAgent nav;
        //"ThirdPersonCharacter" is the name of the script I am using from Unity's Standard Assests, I reference it here so I can use it with my AI script
        public ThirdPersonCharacter enemy;

        public State state;
        private bool alive;

        //Variables for patrolling
        public GameObject[] moveSpots;
        public int moveSpotsID;
        private float patrolSpeed = 0.5f;

        //Variables for chasing
        private float chaseSpeed = 1f;
        private GameObject target;

        //variables for sight
        private float sightDist = 7;
        private float FieldOfView = 22;
        private float DegreeOffset = 90;
        private MeshFilter LOSMeshFilter;
        private Vector3 origin;
        private int rayCount = 40;

        public enum State
        {
            PATROL,
            CHASE,
        }

        void Start()
        {
            LOSMeshFilter = GetComponent<MeshFilter>();
            nav = GetComponent<NavMeshAgent>();
            enemy = GetComponent<ThirdPersonCharacter>();

            nav.updatePosition = true;
            nav.updateRotation = false;

            origin = Vector3.one - Vector3.right;

            moveSpots = GameObject.FindGameObjectsWithTag("Movespot");
            moveSpotsID = Random.Range(0, moveSpots.Length);

            state = randomAI.State.PATROL;
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

                    case State.CHASE:
                        Chase();
                        break;
                }
                yield return null;
            }
        }

        //In my Patrol method I set the movespots on the NavMesh and use an if/else statement to determine which movespot will be the AI's next destination
        void Patrol()
        {
            nav.speed = patrolSpeed;
            if (Vector3.Distance(this.transform.position, moveSpots[moveSpotsID].transform.position) >= 2)
            {
                nav.SetDestination(moveSpots[moveSpotsID].transform.position);
                enemy.Move(nav.desiredVelocity);
            }
            else if (Vector3.Distance(this.transform.position, moveSpots[moveSpotsID].transform.position) <= 2)
            {
                moveSpotsID = Random.Range(0, moveSpots.Length);
            }
            else
            {
                enemy.Move(Vector3.zero);
            }
        }

        //In my Chase method ive set the parameters for the AI to increase in speed and chase the target (user) when spotted
        void Chase()
        {
            nav.speed = chaseSpeed;
            nav.SetDestination(target.transform.position);
            enemy.Move(nav.desiredVelocity);
        }

        void FixedUpdate()
        {
            //Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, transform.forward * sightDist, Color.green);
            //Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized * sightDist, Color.green);
            //Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized * sightDist, Color.green);

            //if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDist))
            //{
            //    if (hit.collider.gameObject.tag == "MC")
            //    {
            //        state = randomAI.State.CHASE;
            //        target = hit.collider.gameObject;
            //    }
            //    else
            //    {
            //        state = randomAI.State.PATROL;
            //    }
            //}

            //if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDist))
            //{
            //    if (hit.collider.gameObject.tag == "MC")
            //    {
            //        state = randomAI.State.CHASE;
            //        target = hit.collider.gameObject;
            //    }
            //    else
            //    {
            //        state = randomAI.State.PATROL;
            //    }
            //}

            //if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized, out hit, sightDist))
            //{
            //    if (hit.collider.gameObject.tag == "MC")
            //    {
            //        state = randomAI.State.CHASE;
            //        target = hit.collider.gameObject;
            //    }
            //    else
            //    {
            //        state = randomAI.State.PATROL;
            //    }
            //}

            //In this version instead of having three rays, I have created a single cone shaped ray
            Vector3[] points = new Vector3[rayCount + 1 + 1];

            RaycastHit hit;

            for (int i = 1; i <= rayCount + 1; i++)
            {
                float degrees = (((float)(i - 1) / rayCount) * FieldOfView);
                degrees -= FieldOfView / 2;
                degrees -= transform.parent.rotation.eulerAngles.y;
                degrees += DegreeOffset;

                if (Physics.Raycast(origin, RadiansToVector3(degrees * Mathf.Deg2Rad).normalized, out hit, sightDist, mask))
                {
                    points[i] = hit.point;
                    Debug.Log("hit");

                    //if (hit.collider.gameObject.tag == "MC")
                    //{
                        
                    //    state = randomAI.State.CHASE;
                    //    target = hit.collider.gameObject;
                    //}
                    //else
                    //{
                    //    state = randomAI.State.PATROL;
                    //}
                }
                else
                {
                    points[i] = RadiansToVector3(degrees * Mathf.Deg2Rad).normalized * sightDist;
                }
            }

            LOSMeshFilter.mesh = CreateMeshFromPoints(points);
        }

        //This method creates an angle that I use for my line of sight
        Vector3 RadiansToVector3(float degrees)
        {
            return new Vector3(Mathf.Cos(degrees), 0, Mathf.Sin(degrees));
        }

        //This method creates a visible mesh so the user can visibly see the enemies line of sight
        /// <summary>
        /// The reason the layer mask might not be working is because it needs to be on its own script and added to a gameobject which attaches to the enemy
        /// </summary>
        Mesh CreateMeshFromPoints(Vector3[] points)
        {
            Mesh m = new Mesh();
            m.name = "LOSMesh";

            points[0] = origin;
            m.vertices = points;

            int[] trianglesArray = new int[(m.vertices.Length - 1) * 3];

            int count = 1;
            for (int i = 0; i < trianglesArray.Length - 3; i += 3)
            {
                trianglesArray[i] = count;
                trianglesArray[i + 1] = 0;
                trianglesArray[i + 2] = count + 1;
                count++;
            }

            m.triangles = trianglesArray;

            m.RecalculateNormals();

            return m;
        }
    }
}