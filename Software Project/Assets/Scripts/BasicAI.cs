using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class basicAI : MonoBehaviour
    {
        //I am using a NavMeshAgent to create a moveable area for the AI
        private NavMeshAgent nav;
        //"ThirdPersonCharacter" is the name of the script I am using from Unity's Standard Assests, I reference it here so I can use it with my AI script
        public ThirdPersonCharacter enemy;

        public enum State
        {
            PATROL,
            CHASE,
        }

        public State state;
        private bool alive;

        //Variables for patrolling
        public GameObject[] moveSpots;
        public int moveSpotsID = 0;
        private float patrolSpeed = 0.5f;

        //Variables for chasing
        private float chaseSpeed = 1f;
        private GameObject target;

        //variables for sight
        private float heightMultiplier = 1.36f;
        private float sightDist = 1;

        void Start()
        {
            nav = GetComponent<NavMeshAgent>();
            enemy = GetComponent<ThirdPersonCharacter>();

            nav.updatePosition = true;
            nav.updateRotation = false;

            state = basicAI.State.PATROL;

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
                moveSpotsID += 1;

                if (moveSpotsID >= moveSpots.Length)
                {
                    moveSpotsID = 0;
                }
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

        void Update()
        {
            RaycastHit hit;

            //Here I am creating three raycasts which work as the AI's vision
            //When the user is caught in a raycast the FSM will kick in and the AI will begin chasing the user, whilst their is no target in sight the AI will go back to patrolling
            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = basicAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
                else
                {
                    state = basicAI.State.PATROL;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = basicAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
                else
                {
                    state = basicAI.State.PATROL;
                }
            }

            if (Physics.Raycast(transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized, out hit, sightDist))
            {
                if (hit.collider.gameObject.tag == "MC")
                {
                    state = basicAI.State.CHASE;
                    target = hit.collider.gameObject;
                }
                else
                {
                    state = basicAI.State.PATROL;
                }
            }
        }
    }
}