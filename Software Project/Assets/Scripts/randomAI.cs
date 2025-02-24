﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace UnityStandardAssets.Characters.ThirdPerson
{

    public class randomAI : MonoBehaviour
    {
        [SerializeField] private LayerMask layerMask;

        //I am using a NavMeshAgent to create a moveable area for the AI
        private NavMeshAgent nav;
        //"ThirdPersonCharacter" is the name of the script I am using from Unity's Standard Assests, I reference it here so I can use it with my AI script
        public ThirdPersonCharacter enemy;

        public State state;
        private bool alive;

        //Variables for patrolling
        public GameObject[] moveSpots;
        public int moveSpotsID = 0;
        private float patrolSpeed = 0.5f;

        //Variables for chasing
        private float chaseSpeed = 0.75f;
        private GameObject target;

        //Variables for investigating
        private float timer = 0;
        private float wait = 4;
        private Vector3 checkSpot;
        private Vector3 investigateSpot;

        //variables for sight
        private float height = 0.5f;
        private float sightDist = 6;
        private float lineOfSight = 30;
        private float startingAngle = 90;
        private int raycast = 1024;
        RaycastHit hit;

        private Mesh mesh;
        private MeshFilter lOSLayer;
        private Material lOSColour;

        public enum State
        {
            PATROL,
            CHASE,
            CHECK,
            INVESTIGATE,
        }

        Vector3 RadiansToVector3(float angle)
        {
            return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        }

        Mesh CreateMeshFromPoints(Vector3[] points)
        {
            mesh = new Mesh();

            points[0] = Vector3.up * height;
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

            nav = GetComponent<NavMeshAgent>();
            enemy = GetComponent<ThirdPersonCharacter>();

            nav.updatePosition = true;
            nav.updateRotation = false;

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

                    case State.CHECK:
                        Check();
                        break;

                    case State.INVESTIGATE:
                        Investigate();
                        break;
                }
                yield return null;
            }
        }

        //In my Patrol method I set the movespots on the NavMesh and use an if/else statement to determine which movespot will be the AI's next destination
        void Patrol()
        {
            lOSColour.color = Color.blue;

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
            timer += Time.deltaTime;
            lOSColour.color = Color.red;

            nav.speed = chaseSpeed;
            nav.SetDestination(target.transform.position);
            enemy.Move(nav.desiredVelocity);

            if (timer >= wait)
            {
                state = randomAI.State.CHECK;
                timer = 0;
            }
        }

        void Check()
        {
            timer += Time.deltaTime;
            lOSColour.color = Color.yellow;

            nav.SetDestination(target.transform.position);
            transform.LookAt(checkSpot);

            if (timer >= wait)
            {
                state = randomAI.State.PATROL;
                timer = 0;
            }
        }

        void Investigate()
        {
            timer += Time.deltaTime;
            lOSColour.color = Color.yellow;

            if (target != null)
            {
                nav.SetDestination(target.transform.position);
                transform.LookAt(target.transform.position);
            }

            if (timer >= wait)
            {
                state = randomAI.State.PATROL;
                timer = 0;
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

                if (Physics.Raycast(transform.position + Vector3.up * height, RadiansToVector3(angle * Mathf.Deg2Rad), out hit, sightDist, layerMask))
                {
                    points[i] = hit.point - transform.position;

                }
                else
                {
                    points[i] = Vector3.up * height + RadiansToVector3(angle * Mathf.Deg2Rad).normalized * sightDist;
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

            Debug.DrawRay(transform.position + Vector3.up * height, transform.forward * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir1).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir2).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir3).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir4).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir5).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir6).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir7).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * height, (transform.forward + dir8).normalized * sightDist, Color.black);

            //Here I am creating three raycasts which work as the AI's vision
            //When the user is caught in a raycast the FSM will kick in and the AI will begin chasing the user, whilst their is no target in sight the AI will go back to patrolling
            if (Physics.Raycast(transform.position + Vector3.up * height, transform.forward, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = randomAI.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir1).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = randomAI.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir2).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = randomAI.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir3).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = randomAI.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir4).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = randomAI.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir5).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = randomAI.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir6).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = randomAI.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir7).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = randomAI.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * height, (transform.forward + dir8).normalized, out hit, sightDist))
            {
                if ((hit.collider.gameObject.tag == "MC") || (hit.collider.gameObject.tag == "MCHidden"))
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }

                if (hit.collider.gameObject.tag == "projectile")
                {
                    state = randomAI.State.INVESTIGATE;
                    target = hit.collider.gameObject;
                }
            }
        }
    }
}