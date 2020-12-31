using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace UnityStandardAssets.Characters.ThirdPerson
{

    public class randomAI : MonoBehaviour
    {
        //I am using a LayerMask to stop the Ai from being able to look through walls
        //public LayerMask mask;
        //I am using a NavMeshAgent to create a moveable area for the AI
        private NavMeshAgent nav;
        //"ThirdPersonCharacter" is the name of the script I am using from Unity's Standard Assests, I reference it here so I can use it with my AI script
        public ThirdPersonCharacter enemy;

        public enum State
        {
            PATROL,
            CHASE,
            INVESTIGATE,
        }

        public State state;
        private bool alive;

        //Variables for patrolling
        public GameObject[] moveSpots;
        public int moveSpotsID;
        private float patrolSpeed = 0.5f;

        //Variables for chasing
        private float chaseSpeed = 0.75f;
        private GameObject target;

        //Variables for investigating
        private float timer = 0;
        private float investigateWait = 4;
        private Vector3 investigateSpot;

        //variables for sight
        private float heightMultiplier = 1.36f;
        private float sightDist = 6;
        RaycastHit hit;

        void Start()
        {
            //LOSMeshFilter = GetComponent<MeshFilter>();
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
            nav.speed = chaseSpeed;
            nav.SetDestination(target.transform.position);
            enemy.Move(nav.desiredVelocity);

            if (timer >= investigateWait)
            {
                state = randomAI.State.INVESTIGATE;
                timer = 0;
            }
        }

        void Investigate()
        {
            timer += Time.deltaTime;
            nav.SetDestination(target.transform.position);
            transform.LookAt(investigateSpot);

            if (timer >= investigateWait)
            {
                state = randomAI.State.PATROL;
                timer = 0;
            }
        }

        void FixedUpdate()
        {
            Vector3 dir1 = Quaternion.Euler(0, 15, 0) * transform.forward;
            Vector3 dir2 = Quaternion.Euler(0, 22.5f, 0) * transform.forward;
            Vector3 dir3 = Quaternion.Euler(0, 30, 0) * transform.forward;
            Vector3 dir4 = Quaternion.Euler(0, 7.5f, 0) * transform.forward;
            Vector3 dir5 = Quaternion.Euler(0, -15, 0) * transform.forward;
            Vector3 dir6 = Quaternion.Euler(0, -22.5f, 0) * transform.forward;
            Vector3 dir7 = Quaternion.Euler(0, -30, 0) * transform.forward;
            Vector3 dir8 = Quaternion.Euler(0, -7.5f, 0) * transform.forward;

            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, transform.forward * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir1).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir2).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir3).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir4).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir5).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir6).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir7).normalized * sightDist, Color.black);
            Debug.DrawRay(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir8).normalized * sightDist, Color.black);

            //Here I am creating three raycasts which work as the AI's vision
            //When the user is caught in a raycast the FSM will kick in and the AI will begin chasing the user, whilst their is no target in sight the AI will go back to patrolling
            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir1).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir2).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir3).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir4).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir5).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir6).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir7).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + dir8).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = randomAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
            }
        }
    }
}