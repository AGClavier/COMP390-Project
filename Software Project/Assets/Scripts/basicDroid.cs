﻿using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class basicDroid : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        //I am using a NavMeshAgent to give the AI commands where to move 
        private NavMeshAgent nav;
        //"ThirdPersonCharacter" is the name of the script I am using from Unity's Standard Assests, I reference it here so I can use it with my AI script
        public ThirdPersonCharacter enemy;

        public State state;
        private bool awake;

        //Variables for patrolling
        public GameObject[] movespot;
        public int movespotIndex = 0;
        public float patrolSpeed = 0.5f;

        //Variables for chasing
        private float chaseSpeed = 0.75f;
        private GameObject target;
        private float timer = 0;
        private float wait = 4;

        //Variables for requesting
        private Vector3 spot;
        private int i = 0;
        private bool help;

        //variables for sight
        private float height = 0.5f;
        private float sightDist = 6;
        private float lineOfSight = 30;
        private float startingAngle = 90;
        private int raycast = 40;
        RaycastHit hit;

        //variables for line of sight layer
        private Mesh mesh;
        private MeshFilter lOSLayer;
        private Material lOSColour;

        public enum State
        {
            PATROL,
            CHASE,
            CHECK,
            INVESTIGATE,
            REQUEST,
        }

        //This converts a radian into a Vector3 to be used as the visible raycasts angle
        Vector3 RadiansToVector3(float angle)
        {
            return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        }

        //This creates the dimensions of the visible raycast
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

        public bool Help()
        {
            help = true;

            return help;
        }

        void Start()
        {
            lOSLayer = GetComponent<MeshFilter>();
            lOSColour = GetComponent<MeshRenderer>().material;

            nav = GetComponent<NavMeshAgent>();
            enemy = GetComponent<ThirdPersonCharacter>();

            nav.updatePosition = true;
            nav.updateRotation = false;

            state = basicDroid.State.PATROL;
            awake = true;

            StartCoroutine(FSM());
        }

        //Here I create a finite state machine in which the AI follows
        //When spawned in the AI will go into its PATROL state where they navigate the map according to its choosen destination
        //If the AI engages with the user they will switch to their CHASE state until the user is out of sight or caught, to which the AI will go to its CHECK state before reverting back to the state PATROL
        //The AI can also chase the players projectile in the INVESTIGATE state or run to the players location if spotted on camera  which is the AI's REQUEST state
        IEnumerator FSM()
        {
            while (awake)
            {
                switch (state)
                {
                    case State.PATROL:
                        Patrol();
                        break;

                    case State.CHASE:
                        Chase();
                        break;

                    case State.CHECK:
                        Check();
                        break;

                    case State.INVESTIGATE:
                        Investigate();
                        break;

                    case State.REQUEST:
                        Request();
                        break;
                }
                yield return null;
            }
        }

       //In my Patrol method I set the movespots on the NavMesh and use an if statement to determine which movespot will be the AI's next destination
        void Patrol()
        {
            lOSColour.color = Color.blue;

            nav.speed = patrolSpeed;

            //The if statment will check if the droid is too far from the first movespot and will set the AI's destination to the movespot with the lowest index
            //Next the else if will set the AI's destination to the next movespot index when it reaches the previous index. But if the next index is equal to
            //the movespots max index, it will make the next destination be the movespots zero index
            if (Vector3.Distance(this.transform.position, movespot[movespotIndex].transform.position) >= 2)
            {
                nav.SetDestination(movespot[movespotIndex].transform.position);
                enemy.Move(nav.desiredVelocity);
            }
            else if (Vector3.Distance(this.transform.position, movespot[movespotIndex].transform.position) <= 2)
            {
                movespotIndex += 1;

                if (movespotIndex >= movespot.Length)
                {
                    movespotIndex = 0;
                }
            }
            else
            {
                enemy.Move(Vector3.zero);
            }
        }

        //In my Chase method I have set the parameters for the AI to increase in speed and chase the target (user) when spotted
        void Chase()
        {
            timer += Time.deltaTime;
            lOSColour.color = Color.red;

            nav.speed = chaseSpeed;
            nav.SetDestination(target.transform.position);
            enemy.Move(nav.desiredVelocity);

            if (timer >= wait)
            {
                state = basicDroid.State.CHECK;
                timer = 0;
            }
        }

        //In my check method I have set the parameters for the AI to rotate and look for the target before returning back to its patrol
        void Check()
        {
            timer += Time.deltaTime;
            lOSColour.color = Color.yellow;

            nav.SetDestination(transform.position);
            enemy.Move(new Vector3(0, 0, 0));
            transform.Rotate(new Vector3(0, 0.6f, 0) * 2);

            if (timer >= wait)
            {
                state = basicDroid.State.PATROL;
                timer = 0;
            }
        }

        //In my Investigate method the AI will chase after the projectile shot by the user
        void Investigate()
        {
            timer += Time.deltaTime;
            lOSColour.color = Color.yellow;

            if (target != null)
            {
                nav.SetDestination(target.transform.position);
                transform.LookAt(target.transform.position);
                enemy.Move(nav.desiredVelocity * 2);
            }

            if (timer >= wait)
            {
                state = basicDroid.State.PATROL;
                timer = 0;
            }
        }

        //In my Request method I have set the parameters for the AI to run to the camera the target was spotted on
        void Request()
        {
            timer += Time.deltaTime;
            lOSColour.color = Color.black;

            if (i == 0)
            {
                spot = GameObject.FindWithTag("MC").transform.position;
                i = 1;
            }

            nav.SetDestination(spot);
            enemy.Move(nav.desiredVelocity * 2);
            transform.Rotate(new Vector3(0, 0.6f, 0));

            if (timer >= wait + 10)
            {
                state = basicDroid.State.PATROL;
                timer = 0;
                i = 0;
                help = false;
            }
        }

        void FixedUpdate()
        {
            if (help == true)
            {
                state = basicDroid.State.REQUEST;
            }

            //Below I am creating the AI's visible raycast
            Vector3[] points = new Vector3[raycast + 2];

            for (int i = 0; i <= raycast; i++)
            {
                float angle = (((float)(i - 1) / raycast) * lineOfSight);
                angle -= lineOfSight / 2;
                //angle -= transform.rotation.eulerAngles.y;
                angle += startingAngle;
                
                if (Physics.Raycast(transform.position, RadiansToVector3(angle * Mathf.Deg2Rad), out hit, sightDist, layerMask))
                {
                    points[i] = hit.point - transform.position;
                    Debug.DrawRay(transform.position, RadiansToVector3(angle * Mathf.Deg2Rad) * sightDist, Color.red);
                }
                else
                {
                    points[i] = RadiansToVector3(angle * Mathf.Deg2Rad).normalized * sightDist;
                    Debug.DrawRay(transform.position, RadiansToVector3(angle * Mathf.Deg2Rad) * sightDist, Color.green);
                }
            }

            lOSLayer.mesh = CreateMesh(points);

            Vector3 dir1 = Quaternion.Euler(0, 7.5f, 0) * transform.forward;
            Vector3 dir2 = Quaternion.Euler(0, 15, 0) * transform.forward;
            Vector3 dir3 = Quaternion.Euler(0, 22.5f, 0) * transform.forward;
            Vector3 dir4 = Quaternion.Euler(0, 30, 0) * transform.forward;
            Vector3 dir5 = Quaternion.Euler(0, -7.5f, 0) * transform.forward;
            Vector3 dir6 = Quaternion.Euler(0, -15, 0) * transform.forward;
            Vector3 dir7 = Quaternion.Euler(0, -22.5f, 0) * transform.forward;
            Vector3 dir8 = Quaternion.Euler(0, -30, 0) * transform.forward;
            

            Debug.DrawRay(transform.position + Vector3.up * height, transform.forward * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir1).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir2).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir3).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir4).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir5).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir6).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir7).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir8).normalized * sightDist, Color.black);

            //Here I am creating nine raycasts which work as the AI's vision
            //When the user is caught in a raycast the FSM will kick in and the AI will begin chasing the user, whilst there is no target in sight the AI will go back to patrolling
            //The raycasts can also pick up the users projectile making the AI chase after it
            if (Physics.Raycast(transform.position + Vector3.up * height, transform.forward, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = basicDroid.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = basicDroid.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir1).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = basicDroid.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = basicDroid.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir2).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = basicDroid.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = basicDroid.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir3).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = basicDroid.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = basicDroid.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir4).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = basicDroid.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = basicDroid.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir5).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = basicDroid.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = basicDroid.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir6).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = basicDroid.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = basicDroid.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir7).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = basicDroid.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = basicDroid.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir8).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = basicDroid.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = basicDroid.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }
        }
    }
}