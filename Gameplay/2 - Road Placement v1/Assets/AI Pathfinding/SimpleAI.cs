using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }
}
