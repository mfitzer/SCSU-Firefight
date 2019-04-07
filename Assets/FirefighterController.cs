using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof (NavMeshAgent))]
public class FirefighterController : MonoBehaviour
{
    NavMeshAgent navAgent;
    Animator anim;
    public Pathfinder pathfinder;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    public void setDestination(Vector3 destination)
    {
        navAgent.SetDestination(destination);
    }

    void controlAnimationState()
    {
        Debug.Log("<color=purple>Destination: " + navAgent.destination + "</color>");

        if (navAgent.pathStatus == NavMeshPathStatus.PathComplete) //Destination not reached, path complete, keep walking
        {
            Debug.Log("Remaining distance: " + navAgent.remainingDistance);
            if (navAgent.remainingDistance > 0)
            {
                anim.SetBool("isWalking", true);
                navAgent.isStopped = false;
            }
            else
            {
                anim.SetBool("isWalking", false);
                navAgent.isStopped = true;
            }
        }
        else //Destination reached, stop walking
        {
            anim.SetBool("isWalking", false);
            navAgent.isStopped = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        setDestination(pathfinder.destination);
        controlAnimationState();
    }
}
