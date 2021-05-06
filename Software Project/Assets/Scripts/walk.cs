using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class walk : MonoBehaviour
    {
        private NavMeshAgent nav;
        public ThirdPersonCharacter character;

        public State state;
        private bool alive;

        public GameObject[] moveSpots;
        public int moveSpotsID = 0;
        private float patrolSpeed = 0.45f;

        public enum State
        {
            PATROL,
        }

        void Start()
        {
            nav = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

            state = walk.State.PATROL;
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
                }
                yield return null;
            }
        }

        void Patrol()
        {
            nav.speed = patrolSpeed;

            if (Vector3.Distance(this.transform.position, moveSpots[moveSpotsID].transform.position) >= 2)
            {
                nav.SetDestination(moveSpots[moveSpotsID].transform.position);
                character.Move(nav.desiredVelocity);
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
                character.Move(Vector3.zero);
            }
        }
    }
}