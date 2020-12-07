using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson {

    public class BasicAI : MonoBehaviour
    {
        public NavMeshAgent agent;
        public ThirdPersonCharacter character;

        public enum State
        {
            PATROL,
            CHASE
        }

        public State state;
        private bool alive;

        //Variable for patrolling
        public GameObject[] waypoints;
        public int waypointID = 0;
        public float patrolSpeed = 0.5f;

        //Variable for chasing
        public float chaseSpeed = 1f;
        public GameObject target;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

            agent.updatePosition = true;
            agent.updateRotation = false;

            state = BasicAI.State.PATROL;

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

                    case State.CHASE:
                        Chase();
                        break;
                }
                yield return null;
            }
        }

        void Patrol()
        {
            agent.speed = patrolSpeed;
            if (Vector3.Distance (this.transform.position, waypoints[waypointID].transform.position) >= 2)
            {
                agent.SetDestination(waypoints[waypointID].transform.position);
                character.Move(agent.desiredVelocity);
            }
            else if (Vector3.Distance(this.transform.position, waypoints[waypointID].transform.position) <= 2)
            {
                waypointID += 1;

                if (waypointID > waypoints.Length) 
                {
                    waypointID = 0;
                }
            }
            else
            {
                character.Move(Vector3.zero);
            }
        }

        void Chase()
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(target.transform.position);
            character.Move(agent.desiredVelocity);
        }

        void OnTriggerEnter (Collider col)
        {
            if (col.tag == "MC")
            {
                state = BasicAI.State.CHASE;
                target = col.gameObject;
            }
        }
    }
}
